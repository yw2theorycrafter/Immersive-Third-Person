using UnityEngine;

namespace com.yw2theorycrafter.thirdpersonview {
    class ThirdPersonViewConfig {

        //[Slider("Radius of camera sphere (swimming)", 1, 10, DefaultValue = 3)]
        public float swimDistance = 3;

        //[Slider("Radius of camera sphere (seamoth/prawn)", 1, 10, DefaultValue = 6)]
        public float vehicleDistance = 6;

        //[Slider("Radius of camera sphere (piloting cyclops)", 1, 10, DefaultValue = 1)]
        public float cyclopsDistance = 1;

        //[Toggle("Switch to first person in bases/cyclops"), OnChange(nameof(OnFirstPersonInBaseChange))]
        public bool switchToFirstPersonWhenInside = true;

        //[Slider("Sensitivity (deg/s)", 1, 180, DefaultValue = 90)]
        public float rotationSpeed = 90;

        //[Slider("Camera distance change speed (m/s)", 0.1f, 10, DefaultValue = 5)]
        public float cameraDistanceDelta = 5;

        //[Slider("Focus area radius", 0, 1.5f, DefaultValue = 0.4f)]
        public float focusRadius = 0.4f;

        //[Slider("Focus centering strength", 0, 1, DefaultValue = 0.7f)]
        public float focusCentering = 0.7f;

        //[Slider("Delay of camera aligning to movement", 0, 10, DefaultValue = 3)]
        public float alignDelay = 3;

        //[Slider("Range of camera alignment (deg)", 0, 90, DefaultValue = 45)]
        public float alignSmoothRange = 45f;
    }
}
