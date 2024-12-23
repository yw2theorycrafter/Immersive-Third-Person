using HarmonyLib;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch(typeof(Player), nameof(Player.SetHeadVisible))]
    class PlayerController_SetHeadVisiblePatch {
        public static bool Prefix(Player __instance)
        {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            var ret = true;
            if (thirdPersonControl && thirdPersonControl.enabled) {
                __instance.head.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                ret = false;
            }
#if DEBUG
            Plugin.Logger.LogInfo($"SetHeadVisible {ret}");
#endif
            return ret;
        }
    }
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    class PlayerController_StartPatch {
        public static void Postfix(Player __instance) {
            __instance.SetHeadVisible(true);
        }
    }
}
