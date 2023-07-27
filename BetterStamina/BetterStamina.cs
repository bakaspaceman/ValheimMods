using BepInEx;
using BepInEx.Configuration;
using System.Reflection;

namespace BetterStamina
{
  [BepInPlugin("bakaSpaceman.BetterStamina", "Better Stamina", "2.0.0")]
    public class BetterStaminaPlugin : BepInPluginTemplate
    {
        // Config - Debug
        static public ConfigEntry<bool> enableStaminaLogging;
        static public ConfigEntry<bool> enableStaminaRegenLogging;
        static public ConfigEntry<bool> enableSkillStaminaLogging;

        // Config - General
        static public ConfigEntry<int> nexusID;
        static public ConfigEntry<bool> removeEncumberedStaminaDrain;
        static public ConfigEntry<float> staminaRegenRateMultiplier;
        static public ConfigEntry<float> staminaRegenRateMultiplierWithWeapons;
        static public ConfigEntry<float> staminaRegenDelay;

        // Config - Tools
        static public ConfigEntry<bool> removeHammerStaminaCost;
        static public ConfigEntry<bool> removeCultivatorStaminaCost;
        static public ConfigEntry<bool> removeHoeStaminaCost;

        // Config - Skills
        static public ConfigEntry<float> runMaxSkillStaminaCost;
        static public ConfigEntry<float> runWithWeapMaxSkillStaminaCost;
        static public ConfigEntry<float> dodgeMaxSkillStaminaCost;
        static public ConfigEntry<float> jumpMaxSkillStaminaCost;
        static public ConfigEntry<float> blockMaxSkillStaminaCost;
        static public ConfigEntry<float> sneakMaxSkillStaminaCost;
        static public ConfigEntry<float> weaponMaxSkillAttackStaminaCost;
        static public ConfigEntry<float> bowMaxSkillHoldStaminaCost;
        static public ConfigEntry<float> swimMaxStaminaCost;
        static public ConfigEntry<float> swimMinStaminaCost;

        // Config - Status Effects
        static public ConfigEntry<float> restedStaminaRegenMultiplier;
        static public ConfigEntry<float> restedDurationPerComfortLvl;
        static public ConfigEntry<float> wetStaminaRegenMultiplier;
        static public ConfigEntry<float> coldStaminaRegenMultiplier;

        // Common use private fields
        static public FieldInfo playerSkillsField = typeof(Player).GetField("m_skills", BindingFlags.Instance | BindingFlags.NonPublic);
        static public FieldInfo statusEffectsField = typeof(SEMan).GetField("m_statusEffects", BindingFlags.Instance | BindingFlags.NonPublic);

        private void SetupConfig()
        {
            nexusID =                           Config.Bind("General",      "NexusID",                      153,    "Nexus mod ID for updates");
            staminaRegenRateMultiplier =        Config.Bind("General",      "StaminaRegenRateMod",          1.5f,   "1 - Default rate, 1.5 - 50% faster rate, 0.5 - 50% slower, etc.");
            staminaRegenRateMultiplierWithWeapons = Config.Bind("General",  "StaminaRegenRateModWithWeapons", 1.4f, "This will be used instead of StaminaRegenRateMod if player has weapons equipped. 1 - Default rate, 1.5 - 50% faster rate, 0.5 - 50% slower, etc.");
            staminaRegenDelay =                 Config.Bind("General",      "StaminaRegenDelay",            1f,     "Time in seconds before stamina starts regenerating. Default value - 1.");
            removeEncumberedStaminaDrain =      Config.Bind("General",      "RemoveEncumberedStaminaDrain", true,   "Prevents stamina drain while encumbered.");

            removeHammerStaminaCost =           Config.Bind("Tools",        "RemoveHammerStaminaCost",      true,   "Repairing and constructing items will not consume stamina.");
            removeHoeStaminaCost =              Config.Bind("Tools",        "RemoveHoeStaminaCost",         true,   "Using hoe will not consume stamina.");
            removeCultivatorStaminaCost =       Config.Bind("Tools",        "RemoveCultivatorStaminaCost",  true,   "Using cultivator will not consume stamina.");

            runMaxSkillStaminaCost =            Config.Bind("Skills",       "RunCostAtMaxSkill",            0.4f,   "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            runWithWeapMaxSkillStaminaCost =    Config.Bind("Skills",       "RunWithWeaponsCostAtMaxSkill", 0.5f,   "This will be used instead of RunCostModifierAtMaxSkill if player has weapons equipped. The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            dodgeMaxSkillStaminaCost =          Config.Bind("Skills",       "DodgeCostAtMaxSkill",          0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            jumpMaxSkillStaminaCost =           Config.Bind("Skills",       "JumpCostAtMaxSkill",           0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            blockMaxSkillStaminaCost =          Config.Bind("Skills",       "BlockCostAtMaxSkill",          0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            sneakMaxSkillStaminaCost =          Config.Bind("Skills",       "SneakCostAtMaxSkill",          0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            weaponMaxSkillAttackStaminaCost =   Config.Bind("Skills",       "WeaponAttackCostAtMaxSkill",   0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            bowMaxSkillHoldStaminaCost =        Config.Bind("Skills",       "HoldBowCostAtMaxSkill",        0.67f,  "The value is a percentage modifier of the default cost. 1 - default cost, < 1 - reduced cost, > 1 - increased");
            swimMaxStaminaCost =                Config.Bind("Skills",       "SwimCostAtMinSkill",           6f,     "Swimming stamina cost defined in units at minimum Swim skill.");
            swimMinStaminaCost =                Config.Bind("Skills",       "SwimCostAtMaxSkill",           3f,     "Swimming stamina cost defined in units at maximum Swim skill.");

            coldStaminaRegenMultiplier =        Config.Bind("Status Effects",   "ColdStaminaRegenModifier",         0.75f,  "Vanilla value - 0.75 (25% penalty)");
            restedStaminaRegenMultiplier =      Config.Bind("Status Effects",   "RestedStaminaRegenModifier",       1.5f,   "Vanilla value - 2 (100% bonus)");
            restedDurationPerComfortLvl =       Config.Bind("Status Effects",   "RestedDurationIncreasePerConfortLevel", 60f,   "This amount of seconds will be added to the effects duration per comfort level. Vanilla value - 60 seconds");
            wetStaminaRegenMultiplier =         Config.Bind("Status Effects",   "WetStaminaRegenModifier",          0.85f,  "Vanilla value - 0.85 (15% penalty)");

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
            harmonyInst.PatchAll(typeof(StatusEffectPatches));

#if DEBUG
            // This is to refresh the values on reloading the mod with F6

            ObjectDB objectDB = BepInExHelpers.FindObjectDB();
            if (objectDB != null)
            {
                StatusEffectPatches.UpdateEffects(objectDB);
            }
#endif
        }
    }
}
