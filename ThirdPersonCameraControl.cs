using UnityEngine;
using UnityEngine.UIElements;

namespace com.yw2theorycrafter.thirdpersonview
{
    class ThirdPersonCameraControl : MonoBehaviour {

        public static ThirdPersonCameraControl main = null;

        private Transform focusTransform;
        public Transform cameraTransform;
        private Transform viewModelTransform;

        public PlayerBreathBubbles breathBubbles;
        public MesmerizedScreenFX mesmerizedScreenFX;

        private float rotationX;
        private float rotationY;
        public float currentDistance;

        public ThirdPersonViewConfig config;

        //Camera ignores player and all default collision masks
        //Also layer 21 - TODO why?
        private int obstructionMask = (~(1 << 21)) & (~(1 << LayerID.Player)) & LayerID.DefaultCollisionMask;
        private Vector3 cameraHalfExtends;

        private bool UsingPDA = false;

        private bool pilotingAnything;

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

            config = new ThirdPersonViewConfig();

            //Defines the camera collision box to avoid clipping.
            //Values > 0.1f seem to break the collision, values < 0.1f place the camera too close to the wall causing frustrum clipping
            //These values are roughly the size of the player head, that makes sense.
            cameraHalfExtends.y = 0.1f;
            cameraHalfExtends.x = 0.1f;
            cameraHalfExtends.z = 0;

            main = this;
        }

        public void RefreshState() {
            UsingPDA = Player.main.GetPDA().isInUse;
            //TODO support for being 3rd person while piloting
            pilotingAnything = Player.main.isPiloting;
            InsideTightSpace = Player.main.IsInBase() || Player.main.IsInSubmarine();

            //XXX remove this, this is for debugging only:
            if (!config.enabled)
            {
                enabled = false;
                return;
            }

            //Unfortunately, exiting from the PDA view causes a little animation glitch, but it's better than seeing the MainCameraControllerPatch for a split second
            bool shouldEnable = config.enabled && !UsingPDA && !pilotingAnything;
            if (config.switchToFirstPersonWhenInside)
            {
                shouldEnable = shouldEnable && !InsideTightSpace;
            }
            if (mesmerizedScreenFX && mesmerizedScreenFX.enabled)
            {
                shouldEnable = false;
            }
            enabled = shouldEnable;
            MainCameraControl.main.enabled = !enabled;

            if (breathBubbles)
            {
                breathBubbles.enabled = !enabled;
            }

            if (!enabled)
            {
                rotationX = 0;
                rotationY = cameraTransform.rotation.eulerAngles.y;
            }
        }

        private void Update() {
            // assert fps input module
            if (FPSInputModule.current == null) return;

            RefreshState();
            if (!enabled)
            {
                return;
            }
            
            Vector3 lookPosition;
            Quaternion lookRotation;

            if (MainCameraControl.main.cinematicMode)
            {
                lookRotation = cameraTransform.rotation;
                rotationX = 0;
                rotationY = lookRotation.eulerAngles.y;
            } else {
                //Handles the start menu?
                if (!FPSInputModule.current.lockRotation)
                {
                    ManualRotation();
                }
                ConstrainAngles();
                lookRotation = Quaternion.Euler(rotationX, rotationY, 0);
            }

            Vector3 lookDirection = lookRotation * Vector3.forward;

            Vector3 castFrom = cameraTransform.parent.position;
            lookPosition = castFrom - lookDirection * config.swimDistance;

            Vector3 castLine = lookPosition - castFrom;
            float castDistance = castLine.magnitude;
            Vector3 castDirection = castLine / castDistance;

            if (Physics.BoxCast(castFrom, cameraHalfExtends, castDirection, out var hit, lookRotation, castDistance, obstructionMask, QueryTriggerInteraction.Ignore)) {
#if DEBUG
                //Quite noisy, useful for debugging the camera collision:
                //Plugin.Logger.LogInfo($"Hit! {hit.collider} layer={hit.collider.gameObject.layer}");
#endif
                //Prevent clipping into the player's head
                currentDistance = Mathf.Max(0.25f, hit.distance);
                lookPosition = -1 * Vector3.forward * currentDistance;
            } else {
                lookPosition = -1 * Vector3.forward * SmoothMoveToDistance(config.swimDistance);
            }

            cameraTransform.localPosition = lookPosition;
            cameraTransform.localEulerAngles = Vector3.zero;

            if (!MainCameraControl.main.cinematicMode)
            {
                UpdateViewModel();
            } else {
                viewModelTransform.localEulerAngles = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
        }

        private float SmoothMoveToDistance(float newDistance) {
            currentDistance = Mathf.MoveTowards(currentDistance, newDistance, Time.deltaTime * config.cameraDistanceDelta);
            return currentDistance;
        }

        private void ManualRotation() {
            Vector2 input = GameInput.GetLookDelta();
            rotationX -= config.rotationSpeed * Time.unscaledDeltaTime * input.y * Mathf.Rad2Deg;
            rotationY += config.rotationSpeed * Time.unscaledDeltaTime * input.x * Mathf.Rad2Deg;
        }

        // Updates this component's transform, as well as player viewmodel
        private void UpdateViewModel() {

            var headingAngles = new Vector3(rotationX, rotationY);

            //This rotates the player
            viewModelTransform.localEulerAngles = Vector3.up * headingAngles.y;
            Player.main.transform.localEulerAngles = Vector3.zero;
            //This rotates the camera
            transform.localEulerAngles = headingAngles;
        }

        private void ConstrainAngles()
        {
            rotationX = Mathf.Clamp(rotationX % 360, -80, 80);
            rotationY %= 360;
        }

        private static float GetAngle(Vector2 direction) {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return direction.x > 0 ? angle : 360 - angle;
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
