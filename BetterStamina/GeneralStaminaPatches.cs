using BetterStamina;
using HarmonyLib;

internal static class GeneralStaminaPatches
{
    [HarmonyPatch(typeof(Player), "UpdateStats")]
    [HarmonyPrefix]
    private static void Prefix(Player __instance)
    {
        __instance.m_encumberedStaminaDrain = 0f;
        __instance.m_staminaRegenTimeMultiplier = BetterStaminaPlugin.staminaRegenRateMultiplier.Value;
    }
}
