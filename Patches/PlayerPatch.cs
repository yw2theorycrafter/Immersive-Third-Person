using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(Player), nameof(Player.SetScubaMaskActive))]
    class Player_SetScubaMaskActivePatch {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance) {
            var comp = __instance.GetComponentInChildren<ThirdPersonCameraControl>();
            return comp is null;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    class Player_UpdatePatch {

        [HarmonyPostfix]
        public static void Postfix(Player __instance) {
            ThirdPersonCameraControl.main.SetInsideTightSpace(__instance.IsInBase() || __instance.IsInSubmarine());
        }
    }
}
