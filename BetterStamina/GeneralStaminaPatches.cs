using BetterStamina;
using HarmonyLib;
using System.Diagnostics;
using UnityEngine;

internal static class GeneralStaminaPatches
{
    private static float defaultEncumberedStaminaDrain = 0f;

    private static void UpdateEncumberedStaminaDrain(Player __instance)
    {
        if (BetterStaminaPlugin.removeEncumberedStaminaDrain.Value)
        {
            if (defaultEncumberedStaminaDrain == 0f)
            {
                defaultEncumberedStaminaDrain = __instance.m_encumberedStaminaDrain;
            }

            __instance.m_encumberedStaminaDrain = 0f;
        }
        else if (defaultEncumberedStaminaDrain != 0f)
        {
            __instance.m_encumberedStaminaDrain = defaultEncumberedStaminaDrain;
            defaultEncumberedStaminaDrain = 0f;
        }
    }

    private static float CalculateNewStamina(Player __instance, bool ___m_wallRunning, float ___m_staminaRegen, float ___m_stamina, SEMan ___m_seman, ref float ___m_staminaRegenTimer, float dt)
    {
        float num1 = 1f;
        if (__instance.IsBlocking())
            num1 *= 0.8f;
        if (((__instance.IsSwimming() && !__instance.IsOnGround() || (__instance.InAttack() || __instance.InDodge()) ? 1 : (___m_wallRunning ? 1 : 0)) | (__instance.IsEncumbered() ? 1 : 0)) != 0)
            num1 = 0.0f;

        float maxStamina = __instance.GetMaxStamina();
        float missingStaminaMod = (float)(1.0 - ___m_stamina / (double)maxStamina);
        float num2 = (___m_staminaRegen + missingStaminaMod * ___m_staminaRegen * __instance.m_staminaRegenTimeMultiplier) * num1;
        float staminaMultiplier = 1f;
        ___m_seman.ModifyStaminaRegen(ref staminaMultiplier);
        float num3 = num2 * staminaMultiplier;
        ___m_staminaRegenTimer -= dt;

        float returnStamina = ___m_stamina;
        if (___m_stamina < (double)maxStamina && ___m_staminaRegenTimer <= 0.0)
            returnStamina = Mathf.Min(maxStamina, ___m_stamina + num3 * dt);

        float staminaChange = returnStamina - ___m_stamina;
        if (Mathf.Abs(staminaChange) > 0f)
        {
            BepInPluginTemplate.DebugLog($"StaminaChangeThisFrame: {num3}(dt-{staminaChange}), base regen - {___m_staminaRegen}; activity mult - {num1}; base mult - {__instance.m_staminaRegenTimeMultiplier}; missing mult - {missingStaminaMod}; SE mult - {staminaMultiplier}");
        }

        return returnStamina;
    }

    [HarmonyPatch(typeof(Player), "UpdateStats")]
    [HarmonyPrefix]
    private static void UpdateStats_Prefix(Player __instance, float dt, SEMan ___m_seman, bool ___m_wallRunning, float ___m_staminaRegen, float ___m_stamina, float ___m_staminaRegenTimer)
    {
        UpdateEncumberedStaminaDrain(__instance);

        __instance.m_staminaRegenTimeMultiplier = __instance.GetCurrentWeapon() != null ? BetterStaminaPlugin.staminaRegenRateMultiplierWithWeapons.Value : BetterStaminaPlugin.staminaRegenRateMultiplier.Value;
        __instance.m_staminaRegenDelay = BetterStaminaPlugin.staminaRegenDelay.Value;

        if (BetterStaminaPlugin.enableStaminaRegenLogging != null && BetterStaminaPlugin.enableStaminaRegenLogging.Value)
        {
            CalculateNewStamina(__instance, ___m_wallRunning, ___m_staminaRegen, ___m_stamina, ___m_seman, ref ___m_staminaRegenTimer, dt);
        }
    }
}

internal static class DebugStaminaPatches
{
    [HarmonyPatch(typeof(Player), "UseStamina")]
    [HarmonyPrefix]
    private static void UseStamina_Prefix(Player __instance, ref float __state, ref float ___m_stamina)
    {
        __state = ___m_stamina;
    }
    [HarmonyPatch(typeof(Player), "UseStamina")]
    [HarmonyPostfix]
    private static void UseStamina_Postfix(Player __instance, float __state, ref float ___m_stamina)
    {
        if (BetterStaminaPlugin.enableStaminaLogging != null && BetterStaminaPlugin.enableStaminaLogging.Value && (___m_stamina - __state) != 0f)
        {
            BetterStaminaPlugin.DebugLog($"UseStamina(): source - {new StackFrame(2).GetMethod().Name}; change - {___m_stamina - __state}");
        }
    }

//     static private float previousStaminaRate = 0f;
//     [HarmonyPatch(typeof(SE_Stats), "ModifyStaminaRegen")]
//     [HarmonyPrefix]
//     private static void ModifyStaminaRegen_Prefix(SE_Stats __instance, ref float staminaRegen, ref float __state)
//     {
//         __state = staminaRegen;
//     }
//     [HarmonyPatch(typeof(SE_Stats), "ModifyStaminaRegen")]
//     [HarmonyPostfix]
//     private static void ModifyStaminaRegen_Postfix(SE_Stats __instance, ref float staminaRegen, float __state)
//     {
//         if (__state != staminaRegen && previousStaminaRate != staminaRegen)
//         {
//             previousStaminaRate = staminaRegen;
//             if (BetterStaminaPlugin.enableStaminaRegenLogging != null && BetterStaminaPlugin.enableStaminaRegenLogging.Value)
//             {
//                 BetterStaminaPlugin.DebugLog($"ModifyStaminaRegen(): source - {__instance.m_name}; new regen - {staminaRegen}; previous - {__state} multiplier - {__instance.m_staminaRegenMultiplier}");
//             }
//         }
//     }
}
