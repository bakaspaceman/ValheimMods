using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

public class BepInPluginTemplate : BaseUnityPlugin
{
    // Config - Debug
    static public ConfigEntry<bool> enableLogging;
    static public ConfigEntry<bool> enableTranspilerLogging;

    protected BepInPlugin BepInAttr { get; set; }
    static protected Harmony harmonyInst { get; set; }
    static protected BepInPluginTemplate modInst { get; set; }
    static protected new ManualLogSource Logger { get; set; }

    public enum LogMessageType
    { 
        LogInfo,
        LogMessage,
        LogDebug,
        LogWarning,
        LogError,
        LogFatal
    }

    public static void DebugTranspilerLog(object message, LogMessageType msgType = LogMessageType.LogInfo)
    {
        DebugLog(message, msgType, true);
    }

    public static void DebugLog(object message, LogMessageType msgType = LogMessageType.LogInfo, bool transpilerlogs = false)
    {
        if (enableLogging != null && enableLogging.Value)
        {
            if (transpilerlogs && enableTranspilerLogging != null && !enableTranspilerLogging.Value)
                return;

            switch (msgType)
            {
                case LogMessageType.LogMessage:
                    Logger.LogMessage(message);
                    break;
                case LogMessageType.LogDebug:
                    Logger.LogDebug(message);
                    break;
                case LogMessageType.LogWarning:
                    Logger.LogWarning(message);
                    break;
                case LogMessageType.LogError:
                    Logger.LogError(message);
                    break;
                case LogMessageType.LogFatal:
                    Logger.LogFatal(message);
                    break;
                default:
                    Logger.LogInfo(message);
                    break;
            }

        }
    }

    public static void PrintOutInstructions(List<CodeInstruction> instructions)
    {
        DebugTranspilerLog($"");
        DebugTranspilerLog($"#############################################################");
        DebugTranspilerLog($"######## MODIFIED INSTRUCTIONS - {instructions.Count} ########");
        DebugTranspilerLog($"#############################################################");
        DebugTranspilerLog($"");

        for (var i = 0; i < instructions.Count; i++)
        {
            CodeInstruction instr = instructions[i];

            DebugTranspilerLog($"{i} {instr}");
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