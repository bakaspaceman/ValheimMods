using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WetAndCold
{
    [BepInPlugin("bakaSpaceman.WetAndCold", "Wet & Cold", "2.0.0")]
    public class WetAndCold : BaseUnityPlugin
    {
        static public ConfigEntry<int> nexusID;
        static public ConfigEntry<float> coldHealthRegenMultiplier;
        static public ConfigEntry<float> wetHealthRegenMultiplier;

        static FieldInfo semanField = typeof(Character).GetField("m_seman", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo statusEffectsField = typeof(SEMan).GetField("m_statusEffects", BindingFlags.Instance | BindingFlags.NonPublic);

        void Awake()
        {
            nexusID = Config.Bind("General", "NexusID", 157, "Nexus mod ID for updates");
            coldHealthRegenMultiplier = Config.Bind("General", "ColdHealthRegenMultiplier", 0.8f, "Percentage multiplier, 0.5 - default value.");
            wetHealthRegenMultiplier = Config.Bind("General", "WetHealthRegenMultiplier", 0.9f, "Percentage multiplier, 0.75 - default value.");

            Harmony.CreateAndPatchAll(typeof(WetAndCold));

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

        private static void UpdateStatusEffect(StatusEffect se, bool onPlayer = false)
        {
            string playerString = onPlayer ? " on local player " : " in ObjectDB ";
            if (se is SE_Stats stats)
            {
                switch (se.m_name)
                {
                    case "$se_cold_name":
#if DEBUG
                        UnityEngine.Debug.Log($"[WetAndCold] [{Localization.instance.Localize(se.m_name)}] Updating m_healthRegenMultiplier{playerString}from {stats.m_healthRegenMultiplier} to {coldHealthRegenMultiplier.Value}");
#endif
                        stats.m_healthRegenMultiplier = coldHealthRegenMultiplier.Value;
                        break;
                    case "$se_wet_name":
#if DEBUG
                        UnityEngine.Debug.Log($"[WetAndCold] [{Localization.instance.Localize(se.m_name)}] Updating m_healthRegenMultiplier{playerString}from {stats.m_healthRegenMultiplier} to {wetHealthRegenMultiplier.Value}");
#endif
                        stats.m_healthRegenMultiplier = wetHealthRegenMultiplier.Value;
                        break;
                }
            }
        }

        private static void UpdateEffects(ObjectDB __instance)
        {
            foreach (StatusEffect se in __instance.m_StatusEffects)
            {
                UpdateStatusEffect(se);
            }
        }

        [HarmonyPatch(typeof(ObjectDB),"Awake")]
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
                    List<StatusEffect> statusEffects = (List<StatusEffect>)statusEffectsField.GetValue(___m_seman);
                    foreach (StatusEffect se in statusEffects)
                    {
                        UpdateStatusEffect(se, true);
                    }
                }
            }
        }
    }
}
