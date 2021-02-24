using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace BetterStamina
{
    [BepInPlugin("bakaSpaceman.BetterStamina", "Better Stamina", "1.0.0.0")]
    public class BetterStaminaPlugin : BaseUnityPlugin
    {
        // Config - Debug
        static public ConfigEntry<bool> enableLogging;
        static public ConfigEntry<bool> enableStaminaLogging;
        static public ConfigEntry<bool> enableStaminaRegenLogging;

        // Config - Stamina
        static public ConfigEntry<bool> enableEncumberedStaminaDrain;
        static public ConfigEntry<float> staminaRegenRateMultiplier;

        // Config - Tools
        static public ConfigEntry<bool> enableHammerStaminaCost;
        static public ConfigEntry<bool> enableCultivatorStaminaCost;
        static public ConfigEntry<bool> enableHoeStaminaCost;

        // Config - Skills
        static public ConfigEntry<float> dodgeMaxSkillStaminaCost;
        static public ConfigEntry<float> jumpMaxSkillStaminaCost;
        static public ConfigEntry<float> blockMaxSkillStaminaCost;
        static public ConfigEntry<float> sneakMaxSkillStaminaCost;
        static public ConfigEntry<float> weaponMaxSkillAttackStaminaCost;

        // Common use private fields
        static public FieldInfo playerSkillsField = typeof(Player).GetField("m_skills", BindingFlags.Instance | BindingFlags.NonPublic);

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
            enableStaminaRegenLogging = Config.Bind("Debug", "StaminaRegenLogging", false, "");

            staminaRegenRateMultiplier = Config.Bind("General", "StaminaRegenRateModifier", 1.3f, "1f = Default rate, 1.5f = 50% faster rate, 0.5f = 50% slower, etc.");
            enableEncumberedStaminaDrain = Config.Bind("General", "EncumberedStaminaDrain", false, "Prevents stamina drain while encumbered.");

            enableHammerStaminaCost = Config.Bind("Tools", "HammerStaminaCost", false, "Repairing and constructing items will not consume stamina.");
            enableHoeStaminaCost = Config.Bind("Tools", "HoeStaminaCost", false, "Using hoe terrain features will not consume stamina.");
            enableCultivatorStaminaCost = Config.Bind("Tools", "CultivatorStaminaCost", false, "Using cultivator terrain features will not consume stamina.");

            dodgeMaxSkillStaminaCost = Config.Bind("Skills", "MaxDodgeSkillStaminaCost", 0.75f, "0.75f = dodging will cost 25% less when at max Jump skill.");
            jumpMaxSkillStaminaCost = Config.Bind("Skills", "MaxJumpSkillStaminaCost", 0.75f, "0.75f = jumping will cost 25% less when at max Jump skill.");
            blockMaxSkillStaminaCost = Config.Bind("Skills", "MaxBlockSkillStaminaCost", 0.75f, "0.75f = blocking will cost 25% less when at max Block skill.");
            sneakMaxSkillStaminaCost = Config.Bind("Skills", "MaxSneakSkillStaminaCost", 0.75f, "0.75f = sneaking will cost 25% less when at max Block skill.");
            weaponMaxSkillAttackStaminaCost = Config.Bind("Skills", "MaxWeaponSkillAttackStaminaCost", 0.67f, "0.67f = attacking will cost 33% less when at max corresponding Weapon skill.");

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
