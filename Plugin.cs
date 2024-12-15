using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace com.yw2theorycrafter.thirdpersonview
{
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.yw2theorycrafter.thirdpersonview";
        private const string PluginName = "Third Person View";
        private const string VersionString = "2.0.0";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        private void Awake()
        {
            // plugin startup logic
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{MyGuid}");

            Logger.LogInfo($"Plugin {MyGuid} is loaded!");
        }
    }
}
