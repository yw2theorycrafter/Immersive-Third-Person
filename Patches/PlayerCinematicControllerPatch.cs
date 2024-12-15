using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(PlayerCinematicController), nameof(PlayerCinematicController.UpdatePlayerPosition))]
    class PlayerCinematicController_UpdatePlayerPositionPatch {
        public static bool Prefix(PlayerCinematicController __instance) {
            if (ThirdPersonCameraControl.main != null && ThirdPersonCameraControl.main.enabled) {
                var tf = __instance.player.transform;
                tf.position = __instance.animatedTransform.position;
                tf.rotation = __instance.animatedTransform.rotation;
                return false;
            }
            return true;
        }
    }
}
