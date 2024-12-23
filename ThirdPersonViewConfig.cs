using Nautilus.Commands;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using Nautilus.Options;
using Nautilus.Options.Attributes;


using System;
using UnityEngine;

namespace com.yw2theorycrafter.immersivethirdperson
{
    [Menu("Immersive Third Person")]
    public class ThirdPersonViewConfig : ConfigFile {
        [Toggle("Enabled")]
        public bool enabled = true;

        [Slider("Camera distance", 1, 20, DefaultValue = 6)]
        public float swimDistance = 6;

        //[Slider("Increases the rotation speed of the third-person camera, relative to the in-game mouse/controller sensitivity settings.", 0.1f, 4, DefaultValue = 1)]
        public float rotationSpeed = 1;
    }
}
