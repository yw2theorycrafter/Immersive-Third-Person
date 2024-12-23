using HarmonyLib;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch(typeof(LaunchRocket), nameof(LaunchRocket.Awake))]
    class LaunchRocket_UpdatePatch{
        [HarmonyPostfix]
        public static void Postfix(LaunchRocket __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            if (thirdPersonControl)
            {
                thirdPersonControl.launchRocket = __instance;
            }
        }
    }
}