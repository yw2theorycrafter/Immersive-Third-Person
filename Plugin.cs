using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using static OVRHaptics;

namespace com.yw2theorycrafter.thirdpersonview
{
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.yw2theorycrafter.thirdpersonview";
        private const string PluginName = "Third Person View";
        private const string VersionString = "1.0.0";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static ThirdPersonViewConfig config;

        private void Awake()
        {
            // plugin startup logic
            Logger = base.Logger;

            config = OptionsPanelHandler.RegisterModOptions<ThirdPersonViewConfig>();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{MyGuid}");

            Logger.LogInfo($"Plugin {MyGuid} is loaded!");
        }
    }
}
