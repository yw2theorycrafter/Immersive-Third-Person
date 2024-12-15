using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview {
    [HarmonyPatch(typeof(CameraToPlayerManager), nameof(CameraToPlayerManager.EnableHeadCameraController))]
    class CameraToPlayerManager_EnableHeadCameraControllerPatch {
        [HarmonyPostfix]
        public static void Postfix() {
            ThirdPersonCameraControl.cameraToPlayerManagerOverride = true;
            ThirdPersonCameraControl.main.enabled = false;
        }
    }
    [HarmonyPatch(typeof(CameraToPlayerManager), nameof(CameraToPlayerManager.DisableHeadCameraController))]
    class CameraToPlayerManager_DisableHeadCameraControllerPatch {
        [HarmonyPostfix]
        public static void Postfix() {
            MainCameraControl.main.enabled = false;
            ThirdPersonCameraControl.main.enabled = true;
            ThirdPersonCameraControl.cameraToPlayerManagerOverride = false;
        }
    }
}
