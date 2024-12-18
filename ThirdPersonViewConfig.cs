using System;
using UnityEngine;

namespace com.yw2theorycrafter.thirdpersonview {
    class ThirdPersonViewConfig {
        //[Toggle("Enable third person view when available")]
        public bool enabled = true;

        //[Slider("Radius of camera sphere (swimming)", 1, 10, DefaultValue = 3)]
        public float swimDistance = 6;

        //[Toggle("Switch to first person in bases/cyclops"), OnChange(nameof(OnFirstPersonInBaseChange))]
        //Recommend leaving this true.
        public bool switchToFirstPersonWhenInside = true;

        //[Slider("Increases the rotation speed of the third-person camera, relative to the in-game mouse/controller sensitivity settings.", 0.1f, 4, DefaultValue = 1)]
        public float rotationSpeed = 1;

        //[Slider("Camera distance change speed (m/s)", 0.1f, 10, DefaultValue = 5)]
        public float cameraDistanceDelta = 5;
    }
}
