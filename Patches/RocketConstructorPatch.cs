using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(RocketConstructor), nameof(RocketConstructor.Update))]
    class RocketConstructor_UpdatePatch{
        [HarmonyPostfix]
        public static void Postfix(RocketConstructor __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            if (thirdPersonControl)
            {
                thirdPersonControl.rocketConstructor = __instance;
            }
        }
    }
}