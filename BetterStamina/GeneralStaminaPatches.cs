using BetterStamina;
using HarmonyLib;
using System.Diagnostics;

internal static class GeneralStaminaPatches
{
    private static float defaultEncumberedStaminaDrain = 0f;

    private static void UpdateEncumberedStaminaDrain(Player __instance)
    {
        if (!BetterStaminaPlugin.enableEncumberedStaminaDrain.Value)
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

    [HarmonyPatch(typeof(Player), "UpdateStats")]
    [HarmonyPrefix]
    private static void UpdateStats_Prefix(Player __instance)
    {
        UpdateEncumberedStaminaDrain(__instance);

        __instance.m_staminaRegenTimeMultiplier = BetterStaminaPlugin.staminaRegenRateMultiplier.Value;
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
        if (BetterStaminaPlugin.enableStaminaLogging.Value && (___m_stamina - __state) != 0f)
        {
            BetterStaminaPlugin.DebugLog($"UseStamina(): source - {new StackFrame(2).GetMethod().Name}; change - {___m_stamina - __state}");
        }
    }

    static private float previousStaminaRate = 0f;
    [HarmonyPatch(typeof(SE_Stats), "ModifyStaminaRegen")]
    [HarmonyPrefix]
    private static void ModifyStaminaRegen_Prefix(SE_Stats __instance, ref float staminaRegen, ref float __state)
    {
        __state = staminaRegen;
    }
    [HarmonyPatch(typeof(SE_Stats), "ModifyStaminaRegen")]
    [HarmonyPostfix]
    private static void ModifyStaminaRegen_Postfix(SE_Stats __instance, ref float staminaRegen, float __state)
    {
        if (__state != staminaRegen && previousStaminaRate != staminaRegen)
        {
            previousStaminaRate = staminaRegen;
            if (BetterStaminaPlugin.enableStaminaLogging.Value)
            {
                BetterStaminaPlugin.DebugLog($"ModifyStaminaRegen(): source - {__instance.m_name}; new regen - {staminaRegen}; previous - {__state} multiplier - {__instance.m_staminaRegenMultiplier}");
            }
        }
    }
}
