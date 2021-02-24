using BetterStamina;
using HarmonyLib;
using UnityEngine;
using MathUtils;
using System.Diagnostics;

internal static class SkillPatches
{
    private static float defaultBlockStaminaDrain = 0f;
    private static float defaultSneakStaminaDrain = 0f;
    private static float defaultStaminaUsage = 0f;
    private static float defaultJumpStaminaUsage = 0f;

    [HarmonyPatch(typeof(Player), "OnSneaking")]
    [HarmonyPrefix]
    private static void OnSneaking_Prefix(Player __instance, Skills ___m_skills)
    {
        defaultSneakStaminaDrain = __instance.m_sneakStaminaDrain;

        if (Player.m_localPlayer == null || (UnityEngine.Object)Player.m_localPlayer != (UnityEngine.Object)__instance)
            return;

        EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
        float interpFactor = easeFunc(1f, BetterStaminaPlugin.sneakMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump));

        __instance.m_sneakStaminaDrain = __instance.m_sneakStaminaDrain * interpFactor;

        if (BetterStaminaPlugin.enableSkillStaminaLogging.Value)
            BetterStaminaPlugin.DebugLog($"OnSneaking: Usage change: {defaultSneakStaminaDrain} - {__instance.m_sneakStaminaDrain}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.sneakMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump))}; Custom: {interpFactor}; skill: {___m_skills.GetSkillFactor(Skills.SkillType.Jump)};");
    }

    [HarmonyPatch(typeof(Player), "OnSneaking")]
    [HarmonyPostfix]
    private static void OnSneaking_PostFix(Player __instance)
    {
        if (defaultSneakStaminaDrain != 0f)
        {
            __instance.m_sneakStaminaDrain = defaultSneakStaminaDrain;
        }
    }

    [HarmonyPatch(typeof(Player), "OnJump")]
    [HarmonyPrefix]
    private static void OnJump_Prefix(Player __instance, Skills ___m_skills)
    {
        defaultJumpStaminaUsage = __instance.m_jumpStaminaUsage;

        if (Player.m_localPlayer == null || (UnityEngine.Object)Player.m_localPlayer != (UnityEngine.Object)__instance)
            return;

        EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
        float interpFactor = easeFunc(1f, BetterStaminaPlugin.jumpMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump));

        __instance.m_jumpStaminaUsage = __instance.m_jumpStaminaUsage * interpFactor;

        if (BetterStaminaPlugin.enableSkillStaminaLogging.Value)
            BetterStaminaPlugin.DebugLog($"OnJump: Usage change: {defaultJumpStaminaUsage} - {__instance.m_jumpStaminaUsage}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.jumpMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump))}; Custom: {interpFactor}; skill: {___m_skills.GetSkillFactor(Skills.SkillType.Jump)};");
    }

    [HarmonyPatch(typeof(Player), "OnJump")]
    [HarmonyPostfix]
    private static void OnJump_PostFix(Player __instance)
    {
        if (defaultJumpStaminaUsage != 0f)
        {
            __instance.m_jumpStaminaUsage = defaultJumpStaminaUsage;
        }
    }

    [HarmonyPatch(typeof(Player), "UpdateDodge")]
    [HarmonyPrefix]
    private static void UpdateDodge_Prefix(Player __instance, Skills ___m_skills, float ___m_queuedDodgeTimer)
    {
        defaultStaminaUsage = __instance.m_dodgeStaminaUsage;

        if (Player.m_localPlayer == null || (UnityEngine.Object)Player.m_localPlayer != (UnityEngine.Object)__instance)
            return;

        EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
        float interpFactor = easeFunc(1f, BetterStaminaPlugin.dodgeMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump));

        __instance.m_dodgeStaminaUsage = __instance.m_dodgeStaminaUsage * interpFactor;

        if (BetterStaminaPlugin.enableSkillStaminaLogging.Value && 
            (double)___m_queuedDodgeTimer > 0.0 && __instance.IsOnGround() && (!__instance.IsDead() && !__instance.InAttack()) && (!__instance.IsEncumbered() && !__instance.InDodge()))
        {
            BetterStaminaPlugin.DebugLog($"UpdateDoge: Usage change: {defaultStaminaUsage} - {__instance.m_dodgeStaminaUsage}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.dodgeMaxSkillStaminaCost.Value, ___m_skills.GetSkillFactor(Skills.SkillType.Jump))}; Custom: {interpFactor}; skill: {___m_skills.GetSkillFactor(Skills.SkillType.Jump)};");
        }
    }

    [HarmonyPatch(typeof(Player), "UpdateDodge")]
    [HarmonyPostfix]
    private static void UpdateDodge_PostFix(Player __instance)
    {
        if (defaultStaminaUsage != 0f)
        {
            __instance.m_dodgeStaminaUsage = defaultStaminaUsage;
        }
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

            if (BetterStaminaPlugin.enableSkillStaminaLogging.Value)
                BetterStaminaPlugin.DebugLog($"BlockAttack: Usage change: {defaultBlockStaminaDrain} - {__instance.m_blockStaminaDrain}; Mathf.Lerp: {Mathf.Lerp(1f, BetterStaminaPlugin.blockMaxSkillStaminaCost.Value, playerSkills.GetSkillFactor(Skills.SkillType.Blocking))}; Custom: {interpFactor}; skill: {playerSkills.GetSkillFactor(Skills.SkillType.Blocking)};");
        }
    }

    [HarmonyPatch(typeof(Humanoid), "BlockAttack")]
    [HarmonyPostfix]
    private static void BlockAttack_PostFix(Player __instance)
    {
        if (defaultBlockStaminaDrain != 0f)
        {
            __instance.m_blockStaminaDrain = defaultBlockStaminaDrain;
        }
    }

    [HarmonyPatch(typeof(Attack), "GetStaminaUsage")]
    [HarmonyPrefix]
    private static bool GetStaminaUsage_Prefix(Attack __instance, Humanoid ___m_character, float ___m_attackStamina, ItemDrop.ItemData ___m_weapon, ref float __result)
    {
        if (Player.m_localPlayer == null || (UnityEngine.Object)Player.m_localPlayer != (UnityEngine.Object)___m_character)
        {
            // Do default logic for non-local players
            return true;
        }

        if ((double)___m_attackStamina <= 0.0)
        {
            __result = 0.0f;
            return false;
        }

        double attackStamina = (double)___m_attackStamina;
        EasingFunctions.Function easeFunc = EasingFunctions.GetEasingFunction(EasingFunctions.Ease.EaseOutSine);
        float interpFactor = easeFunc(1f, BetterStaminaPlugin.weaponMaxSkillAttackStaminaCost.Value, ___m_character.GetSkillFactor(___m_weapon.m_shared.m_skillType));

        __result = (float)(attackStamina * interpFactor);

        if (BetterStaminaPlugin.enableSkillStaminaLogging.Value)
        {
            string callingMethodName = new StackFrame(2).GetMethod().Name;
            if (callingMethodName.Contains("Update"))
            {
                float originalStaminaReduction = (float)(attackStamina * (1f - BetterStaminaPlugin.weaponMaxSkillAttackStaminaCost.Value /*0.330000013113022 */) * (double)___m_character.GetSkillFactor(___m_weapon.m_shared.m_skillType));
                float originalCalculation = (float)(attackStamina - originalStaminaReduction);
                BetterStaminaPlugin.DebugLog($"Attack.GetStaminaUsage(): Cost - {__result}; Original: {originalCalculation}; Custom: {__result}; skill: {___m_character.GetSkillFactor(___m_weapon.m_shared.m_skillType)}({___m_weapon.m_shared.m_skillType});");
            }
        }

        // Skip original function
        return false;
    }
}
