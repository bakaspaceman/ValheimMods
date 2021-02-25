using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace WetAndCold
{
    [BepInPlugin("bakaSpaceman.WetAndCold", "Wet & Cold", "1.0.0.0")]
    public class WetAndCold : BaseUnityPlugin
    {
        void Awake()
        {
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

        private static void UpdateEffects(ObjectDB __instance)
        {
            foreach (StatusEffect se in __instance.m_StatusEffects)
            {
                if (se is SE_Stats stats)
                {
                    switch (se.m_name)
                    {
                        case "$se_cold_name":
                            stats.m_healthRegenMultiplier = 0.8f;
                            UnityEngine.Debug.Log($"[{Localization.instance.Localize(se.m_name)}] Updating m_healthRegenMultiplier");
                            break;
                        case "$se_wet_name":
                            stats.m_healthRegenMultiplier = 0.9f;
                            UnityEngine.Debug.Log($"[{Localization.instance.Localize(se.m_name)}] Updating m_healthRegenMultiplier");
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ObjectDB),"Awake")]
        [HarmonyPostfix]
        public static void ObjectDBAwake_PostFix(ObjectDB __instance)
        {
            UpdateEffects(__instance);
        }
    }
}
