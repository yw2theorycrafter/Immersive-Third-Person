using UnityEngine;

namespace com.yw2theorycrafter.thirdpersonview
{
    class ThirdPersonCameraControl : MonoBehaviour {

        public static ThirdPersonCameraControl main = null;

        private Transform focusTransform;
        private Transform cameraTransform;
        private Transform viewModelTransform;
        private Transform playerTransform;

        private float rotationX;
        private float rotationY;
        private float lastManualRotationTime;
        private Vector3 focusPoint, focusPointLastFrame;
        private float currentDistance;
        private float skin;

        private ThirdPersonViewConfig config;

        //Camera ignores player and all default collision masks
        //layer 0 had some weird collisions on that layer, but leaving on for now 
        //Also layer 21 - TODO why?
        private int obstructionMask = (~(1 << 21)) & (~(1 << LayerID.Player)) & LayerID.DefaultCollisionMask;
        private Vector3 cameraHalfExtends;

        public bool CinematicMode;

        private bool InVehicle = false;

        private bool UsingPDA = false;

        private bool pilotingAnything;
        private bool pilotingCyclops;
        private bool pilotingPrawn;

        private bool InsideTightSpace = false;

        private void OnEnable() {
            if (!main)
            {
                SetupSingleton();
            }

            if (Player.main != null)
                Player.main.SetHeadVisible(true);
        }

        private void OnDisable() {
            if (Player.main != null)
                Player.main.SetHeadVisible(false);
        }

        private void SetupSingleton()
        {
            //Inside this method, be careful about accessing other singletons.

            focusTransform = transform;

            var mainCameraControl = GetComponent<MainCameraControl>();
            cameraTransform = mainCameraControl.cameraOffsetTransform;
            viewModelTransform = mainCameraControl.viewModel;
            var player = GetComponentInParent<Player>();
            playerTransform = player.transform;

            config = new ThirdPersonViewConfig();

            cameraHalfExtends.y = Camera.main.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView);
            cameraHalfExtends.x = cameraHalfExtends.y * Camera.main.aspect;
            cameraHalfExtends.z = 0;

            skin = mainCameraControl.skin;

            main = this;
        }

        public void RefreshState() {
            UsingPDA = Player.main.GetPDA().isInUse;
            //TODO support for being 3rd person while piloting
            pilotingAnything = Player.main.isPiloting;
            InsideTightSpace = Player.main.IsInBase() || Player.main.IsInSubmarine();

            //Unfortunately, exiting from the PDA view causes a little animation glitch, but it's better than seeing the MainCameraControllerPatch for a split second
            bool shouldEnable = config.enabled && !UsingPDA && !pilotingAnything;
            if (config.switchToFirstPersonWhenInside)
            {
                shouldEnable = shouldEnable && !InsideTightSpace;
            }
            enabled = shouldEnable;
            MainCameraControl.main.enabled = !enabled;
        }

        private void Update() {
            // assert fps input module
            if (FPSInputModule.current == null) return;

            RefreshState();
            if (!enabled)
            {
                return;
            }

            UpdateFocusPoint();

            Vector3 lookPosition;
            Quaternion lookRotation;
            if (!FPSInputModule.current.lockRotation && (ManualRotation() || AutomaticRotation())) {
                ConstrainAngles();
                lookRotation = Quaternion.Euler(rotationX, rotationY, 0);
            } else {
                lookRotation = Quaternion.Euler(rotationX, rotationY, 0);
            }

            Vector3 lookDirection = lookRotation * Vector3.forward;

            lookPosition = focusPoint - lookDirection * config.swimDistance;

            Vector3 rectOffset = lookDirection * Camera.main.nearClipPlane;
            Vector3 rectPosition = lookPosition + rectOffset;
            Vector3 castFrom = focusPoint;
            Vector3 castLine = rectPosition - castFrom;
            float castDistance = castLine.magnitude;
            Vector3 castDirection = castLine / castDistance;

            // using cameraHalfExtends * castDistance gives us a really conservative camera collision handler, avoiding clipping even at max distance.
            if (Physics.BoxCast(castFrom, cameraHalfExtends * castDistance, castDirection, out var hit, lookRotation, castDistance, obstructionMask, QueryTriggerInteraction.Ignore)) {
#if DEBUG
                Plugin.Logger.LogInfo($"Hit! {hit.collider} layer={hit.collider.gameObject.layer}");
#endif
                currentDistance = Mathf.Max(0, hit.distance - Camera.main.nearClipPlane);
                lookPosition = -1 * Vector3.forward * currentDistance;
            } else {
                lookPosition = -1 * Vector3.forward * SmoothMoveToDistance(config.swimDistance);
            }

            //When this is not called, the camera does not move at all.
            if (!CinematicMode)
            {
                cameraTransform.localPosition = lookPosition;
                cameraTransform.localEulerAngles = Vector3.zero;
                UpdateViewModel();
            }
        }

        private float SmoothMoveToDistance(float newDistance) {
            currentDistance = Mathf.MoveTowards(currentDistance, newDistance, Time.deltaTime * config.cameraDistanceDelta);
            return currentDistance;
        }

        private void UpdateFocusPoint() {
            focusPointLastFrame = focusPoint;

            //XXX
            focusPoint = focusTransform.position;

            /*
            Vector3 targetPoint = focusTransform.position;
            if (pilotingCyclops) {
                targetPoint -= focusTransform.forward * 1;
            }

            if (config.focusRadius > 0 && !_usingPda) {
                var distance = Vector3.Distance(targetPoint, focusPoint);
                var t = 1f;
                if (distance > 0.01f && config.focusCentering > 0) {
                    t = Mathf.Pow(1 - config.focusCentering, Time.unscaledDeltaTime);
                }
                if (distance > config.focusRadius) {
                    t = Mathf.Min(t, config.focusRadius / distance);
                }
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            } else {
                focusPoint = targetPoint;
            }
            */
        }

        private bool ManualRotation() {

            Vector2 input = GameInput.GetLookDelta();
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e) {
                rotationX -= config.rotationSpeed * Time.unscaledDeltaTime * input.y;
                rotationY += config.rotationSpeed * Time.unscaledDeltaTime * input.x;
                lastManualRotationTime = Time.unscaledTime;
                return true;
            }
            return false;
        }

        private bool AutomaticRotation()
        {
            if (Time.unscaledTime - lastManualRotationTime < config.alignDelay) {
                return false;
            }

            Vector2 movement = new Vector2(
                focusPoint.x - focusPointLastFrame.x,
                focusPoint.z - focusPointLastFrame.z
            ); // here we're concerned only about rotating about Y axis, so we look only at movement in XZ plane
            float movementDeltaSqr = movement.sqrMagnitude;
            if (movementDeltaSqr < 0.000001f) {
                return false;
            }
            float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
            float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(rotationY, headingAngle));
            float rotationChange = config.rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
            if (deltaAbs < config.alignSmoothRange) {
                rotationChange *= deltaAbs / config.alignSmoothRange;
            } 
            else if (180f - deltaAbs < config.alignSmoothRange) {
                rotationChange *= (180f - deltaAbs) / config.alignSmoothRange;
            }
            rotationY = Mathf.MoveTowardsAngle(rotationY, headingAngle, rotationChange);

            return true;
        }

        // Updates this component's transform, as well as player viewmodel
        private void UpdateViewModel() {

            var headingAngles = new Vector3(rotationX, rotationY);
            //TODO get rid of transform
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.down * skin;
            viewModelTransform.localEulerAngles = Vector3.zero;
            viewModelTransform.localPosition = transform.localPosition;

            // Player transform needs only Y input, since view model, which is its child, is rotated on the X axis via animation
            playerTransform.localEulerAngles = Vector3.up * headingAngles.y;
            transform.localEulerAngles = Vector3.right * headingAngles.x;
        }

        private void ConstrainAngles()
        {
            //This is not causing jerks.

            if (pilotingCyclops) {
                rotationX = Mathf.Clamp(rotationX % 360, -60, 60);
                rotationY = Mathf.Clamp(rotationY % 360, -60 + transform.eulerAngles.y, 60 + transform.eulerAngles.y);
            } else {
                rotationX = Mathf.Clamp(rotationX % 360, -80, 80);
                rotationY %= 360;
            }
        }

        private static float GetAngle(Vector2 direction) {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return direction.x > 0 ? angle : 360 - angle;
        }

        public static Vector2 GetVehicleLookDelta() {
            if (!main || !main.enabled)
            {
                return GameInput.GetLookDelta();
            }
            Vehicle vehicle = Player.main.GetVehicle();
            if (MainCameraControl.main != null && vehicle != null) {
                var thirdPersonControl = MainCameraControl.main.GetComponent<ThirdPersonCameraControl>();
                var headingAngles = new Vector3(thirdPersonControl.rotationX, thirdPersonControl.rotationY);
                var vehicleAngles = vehicle.transform.localEulerAngles;

                return new Vector2(Mathf.DeltaAngle(vehicleAngles.y, headingAngles.y), -Mathf.DeltaAngle(vehicleAngles.x, headingAngles.x)) * Time.deltaTime;
            }
            return Vector2.zero;
        }

        public static Vector3 GetFocusPosition(Transform tf) {
            if (!main || !main.enabled)
            {
                return tf.position;
            }
            return main.focusTransform.position;
        }
        public static Vector3 GetFocusForward(Transform tf)
        {
            if (!main || !main.enabled)
            {
                return tf.forward;
            }
            var pointInFrontOfCamera = Camera.main.transform.TransformPoint(Vector3.forward * (2 + main.currentDistance));
            return (pointInFrontOfCamera - GetFocusPosition(tf)).normalized;
        }
    }
}
