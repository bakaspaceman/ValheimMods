using BetterStamina;
using HarmonyLib;
using UnityEngine;
using MathUtils;

internal static class SkillPatches
{
    private static float defaultStaminaUsage;
    private static float defaultBlockStaminaDrain;

    [HarmonyPatch(typeof(Player), "UpdateDodge")]
    [HarmonyPrefix]
    private static void UpdateDodge_Prefix(Player __instance, Skills ___m_skills)
    {
        defaultStaminaUsage = __instance.m_dodgeStaminaUsage;
        EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
        float interpFactor = easeFunc(1f, BetterStaminaPlugin.dodgeMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump));

        __instance.m_dodgeStaminaUsage = __instance.m_dodgeStaminaUsage * interpFactor;

        //BetterStaminaPlugin.DebugLog($"UpdateDoge: Usage change: {defaultStaminaUsage} - {__instance.m_dodgeStaminaUsage}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.dodgeSkillMaxReduction.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump))}; Custom: {interpFactor}; skill: {___m_skills.GetSkillFactor(Skills.SkillType.Jump)};");
    }

    [HarmonyPatch(typeof(Player), "UpdateDodge")]
    [HarmonyPostfix]
    private static void UpdateDodge_PostFix(Player __instance)
    {
        __instance.m_dodgeStaminaUsage = defaultStaminaUsage;
    }

    [HarmonyPatch(typeof(Humanoid), "BlockAttack")]
    [HarmonyPrefix]
    private static void BlockAttack_Prefix(Humanoid __instance)
    {
        if (Player.m_localPlayer != null && (UnityEngine.Object)Player.m_localPlayer == (UnityEngine.Object)__instance)
        {
            defaultBlockStaminaDrain = __instance.m_blockStaminaDrain;

            Skills playerSkills = (Skills)BetterStaminaPlugin.playerSkillsField.GetValue(Player.m_localPlayer);
            if (playerSkills == null)
                return;

            EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
            float interpFactor = easeFunc(1f, BetterStaminaPlugin.blockMaxSkillStaminaCost.Value, playerSkills.GetSkillFactor(Skills.SkillType.Blocking));

            __instance.m_blockStaminaDrain = __instance.m_blockStaminaDrain * interpFactor;

            //BetterStaminaPlugin.DebugLog($"BlockAttack: Usage change: {defaultBlockStaminaDrain} - {__instance.m_blockStaminaDrain}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.blockMaxSkillStaminaCost.Value, playerSkills.GetSkillFactor(Skills.SkillType.Blocking))}; Custom: {interpFactor}; skill: {playerSkills.GetSkillFactor(Skills.SkillType.Blocking)};");
        }
    }

    [HarmonyPatch(typeof(Humanoid), "BlockAttack")]
    [HarmonyPostfix]
    private static void BlockAttack_PostFix(Player __instance)
    {
        __instance.m_blockStaminaDrain = defaultBlockStaminaDrain;
    }
}
