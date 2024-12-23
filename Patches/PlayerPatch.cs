using HarmonyLib;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch(typeof(Player), nameof(Player.SetScubaMaskActive))]
    class Player_SetScubaMaskActivePatch {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance, bool state) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            var ret = true;
            //Prevent enabling the scuba mask whenever ThirdPersonView is enabled via config.
            if (state && thirdPersonControl && thirdPersonControl.config.enabled)
            {
                ret = false;
            }
#if DEBUG
            Plugin.Logger.LogInfo($"SetScubaMaskActive state={state} {ret}");
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
