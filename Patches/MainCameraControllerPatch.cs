using com.yw2theorycrafter.immersivethirdperson;
using HarmonyLib;

namespace com.yw2theorycrafter.immersivethirdperson {
    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.Awake))]
    class MainCameraControl_AwakePatch {
        [HarmonyPostfix]
        public static void Postfix(MainCameraControl __instance)
        {
            var thirdPersonControl = __instance.GetComponent<ThirdPersonCameraControl>();
            if (!thirdPersonControl)
            {
#if DEBUG
                Plugin.Logger.LogInfo($"Creating ThirdPersonCameraControl");
#endif
                thirdPersonControl = __instance.gameObject.AddComponent<ThirdPersonCameraControl>();
            }
        }
    }

    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.ResetCamera))]
    class MainCameraControl_ResetCameraPatch {
        [HarmonyPrefix]
        public static bool Prefix(MainCameraControl __instance) { 
            var thirdPersonControl = ThirdPersonCameraControl.main;
            //var ret = thirdPersonControl == null || !thirdPersonControl.enabled;
            var ret = true;
#if DEBUG
            Plugin.Logger.LogInfo($"ResetCamera {ret}");
#endif
            return ret;
        }
    }

    [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.LookAt))]
    class MainCameraControl_LookAtPatch {
        [HarmonyPrefix]
        public static bool Prefix(MainCameraControl __instance) {
            var thirdPersonControl = ThirdPersonCameraControl.main;
            var ret = thirdPersonControl == null || !thirdPersonControl.enabled;
#if DEBUG
            Plugin.Logger.LogInfo($"LookAt {ret}");
#endif
            return ret;
        }
    }
}
