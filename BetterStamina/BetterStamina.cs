using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace BetterStamina
{
    [BepInPlugin("bakaSpaceman.BetterStamina", "Better Stamina", "1.0.0.0")]
    public class BetterStaminaPlugin : BepInPluginTempalte
    {
        // Config - Debug
        static public ConfigEntry<bool> enableStaminaLogging;
        static public ConfigEntry<bool> enableStaminaRegenLogging;
        static public ConfigEntry<bool> enableSkillStaminaLogging;

        // Config - Stamina
        static public ConfigEntry<bool> removeEncumberedStaminaDrain;
        static public ConfigEntry<float> staminaRegenRateMultiplier;

        // Config - Tools
        static public ConfigEntry<bool> removeHammerStaminaCost;
        static public ConfigEntry<bool> removeCultivatorStaminaCost;
        static public ConfigEntry<bool> removeHoeStaminaCost;

        // Config - Skills
        static public ConfigEntry<float> dodgeMaxSkillStaminaCost;
        static public ConfigEntry<float> jumpMaxSkillStaminaCost;
        static public ConfigEntry<float> blockMaxSkillStaminaCost;
        static public ConfigEntry<float> sneakMaxSkillStaminaCost;
        static public ConfigEntry<float> weaponMaxSkillAttackStaminaCost;
        static public ConfigEntry<float> bowMaxSkillHoldStaminaCost;
        static public ConfigEntry<float> swimMaxStaminaCost;
        static public ConfigEntry<float> swimMinStaminaCost;

        // Common use private fields
        static public FieldInfo playerSkillsField = typeof(Player).GetField("m_skills", BindingFlags.Instance | BindingFlags.NonPublic);

        private void SetupConfig()
        {
            staminaRegenRateMultiplier =        Config.Bind("General",      "StaminaRegenRateModifier",     1.2f, "1 - Default rate, 1.5 - 50% faster rate, 0.5 - 50% slower, etc.");
            removeEncumberedStaminaDrain =      Config.Bind("General",      "RemoveEncumberedStaminaDrain", true, "Prevents stamina drain while encumbered.");

            removeHammerStaminaCost =           Config.Bind("Tools",        "RemoveHammerStaminaCost",      true, "Repairing and constructing items will not consume stamina.");
            removeHoeStaminaCost =              Config.Bind("Tools",        "RemoveHoeStaminaCost",         true, "Using hoe will not consume stamina.");
            removeCultivatorStaminaCost =       Config.Bind("Tools",        "RemoveCultivatorStaminaCost",  true, "Using cultivator will not consume stamina.");

            dodgeMaxSkillStaminaCost =          Config.Bind("Skills",       "DodgeCostAtMaxSkill",          0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            jumpMaxSkillStaminaCost =           Config.Bind("Skills",       "JumpCostAtMaxSkill",           0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            blockMaxSkillStaminaCost =          Config.Bind("Skills",       "BlockCostAtMaxSkill",          0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            sneakMaxSkillStaminaCost =          Config.Bind("Skills",       "SneakCostAtMaxSkill",          0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            weaponMaxSkillAttackStaminaCost =   Config.Bind("Skills",       "WeaponAttackCostAtMaxSkill",   0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            bowMaxSkillHoldStaminaCost =        Config.Bind("Skills",       "HoldBowCostAtMaxSkill",        0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            swimMaxStaminaCost =                Config.Bind("Skills",       "SwimCostAtMinSkill",           6f,     "Swimming stamina cost defined in units at minimum Swim skill.");
            swimMinStaminaCost =                Config.Bind("Skills",       "SwimCostAtMaxSkill",           3f,     "Swimming stamina cost defined in units at maximum Swim skill.");

#if DEBUG
            enableStaminaLogging =              Config.Bind("Debug",        "StaminaLogging",               false, "");
            enableStaminaRegenLogging =         Config.Bind("Debug",        "StaminaRegenLogging",          false, "");
            enableSkillStaminaLogging =         Config.Bind("Debug",        "SkillLogging",                 false, "");
#endif
        }

        protected override void Awake()
        {
            base.Awake();

            SetupConfig();

            if (enableLogging != null && enableLogging.Value)
            {
                harmonyInst.PatchAll(typeof(DebugStaminaPatches));
            }
            harmonyInst.PatchAll(typeof(GeneralStaminaPatches));
            harmonyInst.PatchAll(typeof(ToolsPatches));
            harmonyInst.PatchAll(typeof(SkillPatches));
        }
    }
}
