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
        static public ConfigEntry<bool> enableEncumberedStaminaDrain;
        static public ConfigEntry<float> staminaRegenRateMultiplier;

        // Config - Tools
        static public ConfigEntry<bool> enableHammerStaminaCost;
        static public ConfigEntry<bool> enableCultivatorStaminaCost;
        static public ConfigEntry<bool> enableHoeStaminaCost;

        // Config - Skills
        static public ConfigEntry<float> dodgeSkillMaxReduction;

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
            enableEncumberedStaminaDrain = Config.Bind("General", "EncumberedStaminaDrain", false, "Prevents stamina drain while encumbered.");

            enableHammerStaminaCost = Config.Bind("Tools", "HammerStaminaCost", false, "Repairing and constructing items will not consume stamina.");
            enableHoeStaminaCost = Config.Bind("Tools", "HoeStaminaCost", false, "Using hoe terrain features will not consume stamina.");
            enableCultivatorStaminaCost = Config.Bind("Tools", "CultivatorStaminaCost", false, "Using cultivator terrain features will not consume stamina.");

            dodgeSkillMaxReduction = Config.Bind("Skills", "MaxDodgeReduction", 0.5f, "0.5f = dodging will cost half as much when at max Jump skill.");

            DebugLog($"PATCHING");

            harmonyInst = new Harmony("bakaSpaceman.BetterStamina");
            if (enableLogging.Value)
            {
                harmonyInst.PatchAll(typeof(DebugStaminaPatches));
            }
            harmonyInst.PatchAll(typeof(GeneralStaminaPatches));
            harmonyInst.PatchAll(typeof(ToolsPatches));
            harmonyInst.PatchAll(typeof(SkillPatches));

            
        }

        void OnDestroy()
        {
            DebugLog($"UNPATCHING");
            harmonyInst.UnpatchSelf();
            BepInEx.Logging.Logger.Sources.Remove(Logger);
        }
    }
}
