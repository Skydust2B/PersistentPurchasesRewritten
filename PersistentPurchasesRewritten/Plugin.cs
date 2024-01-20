using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace PersistentPurchasesRewritten;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource log = new(PluginInfo.PLUGIN_NAME);
    public static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        ConfigManager.configFile = Config;
        BepInEx.Logging.Logger.Sources.Add(log);
        harmony.PatchAll(typeof(Patches));
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}