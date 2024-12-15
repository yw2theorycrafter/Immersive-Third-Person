using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.Awake))]
    class MainCameraControl_AwakePatch {
        [HarmonyPostfix]
        public static void Postfix(MainCameraControl __instance) {
            __instance.gameObject.AddComponent<ThirdPersonCameraControl>();
        }
    }

    /*
    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.Update))]
    class MainCameraControl_UpdatePatch {
        [HarmonyPostfix]
        public static bool Prefix(MainCameraControl __instance) {
            if (ThirdPersonCameraControl.main != null && ThirdPersonCameraControl.main.enabled) {
                __instance.enabled = false;
                return false;
            }
            return true;
        }
    }
    */

    [HarmonyPatch(typeof(MainCameraControl), "set_cinematicMode")]
    class MainCameraControl_SetCinematicModePatch {
        [HarmonyPostfix]
        public static bool Prefix(MainCameraControl __instance, bool value) {
            var thirdPersonControl = __instance.GetComponent<ThirdPersonCameraControl>();
            if (thirdPersonControl && thirdPersonControl.enabled) {
                thirdPersonControl.CinematicMode = value;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.ResetCamera))]
    class MainCameraControl_ResetCameraPatch {
        [HarmonyPostfix]
        public static bool Prefix(MainCameraControl __instance) {
            var thirdPersonControl = __instance.GetComponent<ThirdPersonCameraControl>();
            return thirdPersonControl == null || !thirdPersonControl.enabled;
        }
    }
    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.LookAt))]
    class MainCameraControl_LookAtPatch {
        [HarmonyPostfix]
        public static bool Prefix(MainCameraControl __instance) {
            var thirdPersonControl = __instance.GetComponent<ThirdPersonCameraControl>();
            return thirdPersonControl == null || !thirdPersonControl.enabled;
        }
    }
}
