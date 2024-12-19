using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(MesmerizedScreenFX), nameof(MesmerizedScreenFX.Start))]
    class MesmerizedScreenFX_StartPatch{
        [HarmonyPostfix]
        public static void Postfix(MesmerizedScreenFX __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            if (thirdPersonControl)
            {
                thirdPersonControl.mesmerizedScreenFX = __instance;
            }
        }
    }
}
