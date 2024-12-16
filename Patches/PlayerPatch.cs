﻿using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(Player), nameof(Player.SetScubaMaskActive))]
    class Player_SetScubaMaskActivePatch {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            var ret = true;
            if (thirdPersonControl && thirdPersonControl.enabled)
            {
                ret = false;
            }
#if DEBUG
            Plugin.Logger.LogInfo($"SetScubaMaskActive {ret}");
#endif
            return ret;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    class Player_UpdatePatch {

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            if (thirdPersonControl)
            {
                thirdPersonControl.RefreshState();
            }
        }
    }
}
