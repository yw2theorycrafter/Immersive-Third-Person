using HarmonyLib;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch(typeof(PlayerBreathBubbles), nameof(PlayerBreathBubbles.Start))]
    class PlayerBreathBubbles_StartPatch{
        [HarmonyPostfix]
        public static void Postfix(PlayerBreathBubbles __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            if (thirdPersonControl)
            {
                thirdPersonControl.breathBubbles = __instance;
            }
        }
    }
}
