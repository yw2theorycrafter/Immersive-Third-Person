using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update))]
    class Vehicle_UpdatePatch {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var inputMethod = AccessTools.Method(typeof(GameInput), nameof(GameInput.GetLookDelta));
            
            foreach (var instruction in instructions) {
                if (instruction.Calls(inputMethod)) {
                    instruction.operand = AccessTools.Method(typeof(ThirdPersonCameraControl), nameof(ThirdPersonCameraControl.GetVehicleLookDelta));
                }
                yield return instruction;
            }
        }
    }
}
