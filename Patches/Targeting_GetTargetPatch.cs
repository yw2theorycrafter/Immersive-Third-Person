using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch]
    class Targeting_GetTargetPatch {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var getPosMethod = AccessTools.Method(typeof(Transform), "get_position");
            var getDirMethod = AccessTools.Method(typeof(Transform), "get_forward");
            foreach (var instruction in instructions) {
                if (instruction.Calls(getPosMethod)) {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetFocusPosition)));
                } else if (instruction.Calls(getDirMethod)) {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetFocusForward)));
                } else {
                    yield return instruction;
                }
            }
        }
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() {
            return AccessTools.FirstMethod(typeof(Targeting), method => method.Name.Contains("GetTarget") && method.GetParameters()[0].ParameterType == typeof(float));
        }
    }
}
