using BetterStamina;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

internal static class StatusEffectPatches
{
    private static void UpdateStatusEffect(StatusEffect se, bool onPlayer = false)
    {
        string playerString = onPlayer ? " on local player " : " in ObjectDB ";
        if (se is SE_Stats stats)
        {
            switch (se.m_name)
            {
                case "$se_cold_name":
                    BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_staminaRegenMultiplier{playerString}from {stats.m_staminaRegenMultiplier} to {BetterStaminaPlugin.coldStaminaRegenMultiplier.Value}");
                    stats.m_staminaRegenMultiplier = BetterStaminaPlugin.coldStaminaRegenMultiplier.Value;
                    break;
                case "$se_wet_name":
                    BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_staminaRegenMultiplier{playerString}from {stats.m_staminaRegenMultiplier} to {BetterStaminaPlugin.wetStaminaRegenMultiplier.Value}");
                    stats.m_staminaRegenMultiplier = BetterStaminaPlugin.wetStaminaRegenMultiplier.Value;
                    break;
                case "$se_rested_name":
                    BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_staminaRegenMultiplier{playerString}from {stats.m_staminaRegenMultiplier} to {BetterStaminaPlugin.restedStaminaRegenMultiplier.Value}");
                    stats.m_staminaRegenMultiplier = BetterStaminaPlugin.restedStaminaRegenMultiplier.Value;

                    SE_Rested ser = se as SE_Rested;
                    BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_TTLPerComfortLevel{playerString}from {ser.m_TTLPerComfortLevel} to {BetterStaminaPlugin.restedDurationPerComfortLvl.Value}");
                    ser.m_TTLPerComfortLevel = BetterStaminaPlugin.restedDurationPerComfortLvl.Value;
                    break;
            }
        }
    }

    public static void UpdateEffects(ObjectDB __instance)
    {
        foreach (StatusEffect se in __instance.m_StatusEffects)
        {
            if (se is SE_Stats stats)
            {
                UpdateStatusEffect(se);
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    [HarmonyPostfix]
    public static void ObjectDBAwake_PostFix(ObjectDB __instance)
    {
        UpdateEffects(__instance);
    }

    [HarmonyPatch(typeof(Player), "SetLocalPlayer")]
    [HarmonyPostfix]
    public static void SetLocalPlayerPostFix(Player __instance, SEMan ___m_seman)
    {
        if (__instance != null)
        {
            if (___m_seman != null)
            {
                List<StatusEffect> statusEffects = (List<StatusEffect>)BetterStaminaPlugin.statusEffectsField.GetValue(___m_seman);
                foreach (StatusEffect se in statusEffects)
                {
                    UpdateStatusEffect(se, true);
                }
            }
        }
    }
}
