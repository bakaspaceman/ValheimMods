using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BetterStamina
{
    [BepInPlugin("bakaSpaceman.BetterStamina", "Better Stamina", "1.0.0.0")]
    public class BetterStaminaPlugin : BaseUnityPlugin
    {
        // Config - Debug
        static private ConfigEntry<bool> enableLogging;
        static public ConfigEntry<bool> enableStaminaLogging;

        // Config - Stamina
        static public ConfigEntry<bool> disableEncumberedStaminaDrain;
        static public ConfigEntry<float> staminaRegenRateMultiplier;

        // Config - Tools
        static public ConfigEntry<bool> disableHammerStaminaCost;
        static public ConfigEntry<bool> disableCultivatorStaminaCost;
        static public ConfigEntry<bool> disableHoeStaminaCost;

        static private Harmony harmonyInst;
        static private new ManualLogSource Logger { get; set; }

        public static void DebugLog(object message)
        {
            if (enableLogging.Value)
            {
                Logger.LogInfo(message);
            }
        }

        void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("BetterStamina");

            enableLogging = Config.Bind("Debug", "Logging", false, "");
            enableStaminaLogging = Config.Bind("Debug", "StaminaLogging", false, "");

            staminaRegenRateMultiplier = Config.Bind("General", "StaminaRegenRateModifier", 1.3f, "1f = Default rate, 1.5f = 50% faster rate, 0.5f = 50% slower, etc.");
            disableEncumberedStaminaDrain = Config.Bind("General", "EncumberedStaminaDrain", true, "Prevents stamina drain while encumbered.");

            disableHammerStaminaCost = Config.Bind("Tools", "HammerStaminaCost", true, "Repairing and constructing items will not consume stamina.");
            disableHoeStaminaCost = Config.Bind("Tools", "HoeStaminaCost", true, "Using hoe terrain features will not consume stamina.");
            disableCultivatorStaminaCost = Config.Bind("Tools", "CultivatorStaminaCost", true, "Using cultivator terrain features will not consume stamina.");

            DebugLog($"PATCHING");

            harmonyInst = new Harmony("bakaSpaceman.BetterStamina");
            harmonyInst.PatchAll(typeof(GeneralStaminaPatches));
            harmonyInst.PatchAll(typeof(ToolsPatches));

            if (enableLogging.Value)
            {
                harmonyInst.PatchAll(typeof(DebugStaminaPatches));
            }
        }

        void OnDestroy()
        {
            DebugLog($"UNPATCHING");
            harmonyInst.UnpatchSelf();
            BepInEx.Logging.Logger.Sources.Remove(Logger);
        }
    }
}
