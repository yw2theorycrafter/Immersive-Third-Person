using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch]
    class Utils_TraceFPSTargetPositionPatch {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var positionCall = AccessTools.Method(typeof(Transform), "get_position");
            var fwdCall = AccessTools.Method(typeof(Transform), "get_forward");
            foreach (var instruction in instructions) {
                if (instruction.Calls(positionCall)) {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetFocusPosition)));
                } else if (instruction.Calls(fwdCall)) {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetFocusForward)));
                } else {
                    yield return instruction;
                }
            }
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() {
            return AccessTools.FirstMethod(typeof(UWE.Utils), method => method.Name.Contains("TraceFPSTargetPosition") && method.GetParameters()[4].IsOut);
        }
    }
}
