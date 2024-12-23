using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace com.yw2theorycrafter.immersivethirdperson {
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

    [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.TraceForGrabTarget))]
    class PropulsionCannon_TraceForGrabTargetPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var positionCall = AccessTools.Method(typeof(Transform), "get_position");
            var fwdCall = AccessTools.Method(typeof(Transform), "get_forward");
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(positionCall))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetFocusPosition)));
                }
                else if (instruction.Calls(fwdCall))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetFocusForward)));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
