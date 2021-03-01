using BetterStamina;
using HarmonyLib;
using UnityEngine;

internal static class StatusEffectPatches
{
    public static void OnAwake()
    {
        GameObject gameMainObj = GameObject.Find("_GameMain");
        if (gameMainObj != null)
        {
            ObjectDB objectDB = gameMainObj.GetComponent<ObjectDB>();
            if (objectDB != null)
            {
                UpdateEffects(objectDB);
            }
        }
    }

    public static void UpdateEffects(ObjectDB __instance)
    {
        foreach (StatusEffect se in __instance.m_StatusEffects)
        {
            if (se is SE_Stats stats)
            {
                switch (se.m_name)
                {
                    case "$se_cold_name":
                        BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_staminaRegenMultiplier from {stats.m_staminaRegenMultiplier} to {BetterStaminaPlugin.coldStaminaRegenMultiplier.Value}");
                        stats.m_staminaRegenMultiplier = BetterStaminaPlugin.coldStaminaRegenMultiplier.Value;
                        break;
                    case "$se_wet_name":
                        BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_staminaRegenMultiplier from {stats.m_staminaRegenMultiplier} to {BetterStaminaPlugin.wetStaminaRegenMultiplier.Value}");
                        stats.m_staminaRegenMultiplier = BetterStaminaPlugin.wetStaminaRegenMultiplier.Value;
                        break;
                    case "$se_rested_name":
                        BetterStaminaPlugin.DebugLog($"[{Localization.instance.Localize(se.m_name)}] Updating m_staminaRegenMultiplier from {stats.m_staminaRegenMultiplier} to {BetterStaminaPlugin.restedStaminaRegenMultiplier.Value}");
                        stats.m_staminaRegenMultiplier = BetterStaminaPlugin.restedStaminaRegenMultiplier.Value;
                        break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    [HarmonyPostfix]
    public static void ObjectDBAwake_PostFix(ObjectDB __instance)
    {
        UpdateEffects(__instance);
    }
}
