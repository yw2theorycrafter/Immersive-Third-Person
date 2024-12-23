using Discord;
using HarmonyLib;
using UnityEngine;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch(typeof(Builder), nameof(Builder.GetAimTransform))]
    class Builder_GetAimTransformPatch{
        [HarmonyPrefix]
        public static bool Prefix(ref Transform __result) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            bool ret = true;
            if (thirdPersonControl && thirdPersonControl.enabled)
            {
                __result = thirdPersonControl.transform;
                ret = false;
            }
            return ret;
        }
    }
}
