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

        // Config - Stamina
        static public ConfigEntry<bool> disableEncumberedStaminaDrain;
        static public ConfigEntry<float> staminaRegenRateMultiplier;

        // Config - Tools
        static public ConfigEntry<bool> disableHammerStaminaCost;
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

            staminaRegenRateMultiplier = Config.Bind("General", "StaminaRegenRateModifier", 2f, "1f = Default rate, 1.5f = 50% faster rate, 0.5f = 50% slower, etc.");
            disableEncumberedStaminaDrain = Config.Bind("General", "EncumberedStaminaDrain", true, "Prevents stamina drain while encumbered.");

            disableHammerStaminaCost = Config.Bind("Tools", "HammerStaminaCost", true, "Repairing and constructing items will not consume stamina.");
            disableHoeStaminaCost = Config.Bind("Tools", "HoeStaminaCost", false, "Using hoe terrain features will not consume stamina.");

            DebugLog($"PATCHING");

            harmonyInst = new Harmony("bakaSpaceman.BetterStamina");
            harmonyInst.PatchAll(typeof(GeneralStaminaPatches));
            harmonyInst.PatchAll(typeof(ToolsPatches));
        }

        void OnDestroy()
        {
            DebugLog($"UNPATCHING");
            harmonyInst.UnpatchSelf();
            BepInEx.Logging.Logger.Sources.Remove(Logger);
        }
    }
}
