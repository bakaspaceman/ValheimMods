using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;

public class BepInPluginTemplate : BaseUnityPlugin
{
    // Config - Debug
    static public ConfigEntry<bool> enableLogging;
    static public ConfigEntry<bool> enableTranspilerLogging;

    protected BepInPlugin BepInAttr { get; set; }
    static protected Harmony harmonyInst { get; set; }
    static protected BepInPluginTemplate modInst { get; set; }
    static protected new ManualLogSource Logger { get; set; }

    public static void DebugTranspilerLog(object message)
    {
        DebugLog(message, true);
    }

    public static void DebugLog(object message, bool transpilerlogs = false)
    {
        if (enableLogging != null && enableLogging.Value)
        {
            if (transpilerlogs && enableTranspilerLogging != null && !enableTranspilerLogging.Value)
                return;

            Logger.LogInfo(message);
        }
    }

    protected virtual void Awake()
    {
        modInst = this;

        BepInAttr = (BepInPlugin)Attribute.GetCustomAttribute(GetType(), typeof(BepInPlugin));

        Logger = BepInEx.Logging.Logger.CreateLogSource(BepInAttr.Name);

        harmonyInst = new Harmony(BepInAttr.GUID);

#if DEBUG
        enableLogging = Config.Bind("Debug", "Logging", false, "");
        enableTranspilerLogging = Config.Bind("Debug", "TranspilerLogging", false, "");
#endif

        DebugLog($"Loading..");
    }

    protected virtual void OnDestroy()
    {
        if (harmonyInst != null)
        {
            harmonyInst.UnpatchSelf();
        }

        DebugLog($"Unloading..");
        BepInEx.Logging.Logger.Sources.Remove(Logger);
    }
}