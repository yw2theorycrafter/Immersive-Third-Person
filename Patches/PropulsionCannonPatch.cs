using HarmonyLib;
using UnityEngine;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.UpdateTargetPosition))]
    class PropulsionCannon_StartPatch{
        [HarmonyPostfix]
        public static void Postfix(PropulsionCannon __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            if (thirdPersonControl && thirdPersonControl.enabled)
            {
                __instance.targetPosition = __instance.targetPosition + thirdPersonControl.cameraTransform.rotation * Vector3.forward * thirdPersonControl.currentDistance;
            }
        }
    }
}
