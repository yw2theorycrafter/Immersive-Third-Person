using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(Player), nameof(Player.SetHeadVisible))]
    class PlayerController_SetHeadVisiblePatch {
        public static bool Prefix(Player __instance) {
            if (ThirdPersonCameraControl.main.enabled) {
                __instance.head.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    class PlayerController_StartPatch {
        public static void Postfix(Player __instance) {
            __instance.SetHeadVisible(true);
        }
    }
}
