using BetterStamina;
using HarmonyLib;
using UnityEngine;
using MathUtils;

internal static class SkillPatches
{
    private static float defaultStaminaUsage;

    [HarmonyPatch(typeof(Player), "UpdateDodge")]
    [HarmonyPrefix]
    private static void UpdateDodge_Prefix(Player __instance, Skills ___m_skills)
    {
        defaultStaminaUsage = __instance.m_dodgeStaminaUsage;
        EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
        float interpFactor = easeFunc(1f, BetterStaminaPlugin.dodgeSkillMaxReduction.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump));

        __instance.m_dodgeStaminaUsage = __instance.m_dodgeStaminaUsage * interpFactor;

        //BetterStaminaPlugin.DebugLog($"UpdateDoge: Usage change: {defaultStaminaUsage} - {__instance.m_dodgeStaminaUsage}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.dodgeSkillMaxReduction.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump))}; Custom: {interpFactor}; skill: {___m_skills.GetSkillFactor(Skills.SkillType.Jump)};");
    }

    [HarmonyPatch(typeof(Player), "UpdateDodge")]
    [HarmonyPostfix]
    private static void UpdateDodge_PostFix(Player __instance)
    {
        __instance.m_dodgeStaminaUsage = defaultStaminaUsage;


    }
}
