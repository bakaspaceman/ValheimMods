using BepInEx.Configuration;
using BetterStamina;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

internal static class WeaponStatsPatches
{
    public static void UpdateWeaponStats(ObjectDB __instance)
    {
        foreach (GameObject gameObject in __instance.m_items)
        {
            ItemDrop itemDrop = gameObject.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                if (itemDrop.m_itemData != null)
                {
                    if (itemDrop.m_itemData.m_shared != null)
                    {
                        if (itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon ||
                            itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon ||
                            itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield ||
                            itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow ||
                            itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch ||
                            itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Ammo ||
                            itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool)
                        {
                            if (//itemDrop.gameObject.name.StartsWith("Axe") ||
                                //itemDrop.gameObject.name.StartsWith("Atgeir") ||
                                //itemDrop.gameObject.name.StartsWith("Battleaxe") ||
                                itemDrop.gameObject.name.StartsWith("Bow"))
                                //itemDrop.gameObject.name.StartsWith("Cultivator") ||
                                //itemDrop.gameObject.name.StartsWith("Club") ||
                                //itemDrop.gameObject.name.StartsWith("Hammer") ||
                                //itemDrop.gameObject.name.StartsWith("Hoe") ||
                                //itemDrop.gameObject.name.StartsWith("Knife") ||
                                //itemDrop.gameObject.name.StartsWith("Mace") ||
                                //itemDrop.gameObject.name.StartsWith("Pickaxe") ||
                                //itemDrop.gameObject.name.StartsWith("Shield") ||
                                //itemDrop.gameObject.name.StartsWith("Sledge") ||
                                //itemDrop.gameObject.name.StartsWith("Spear") ||
                                //itemDrop.gameObject.name.StartsWith("Sword") ||
                                //itemDrop.gameObject.name.StartsWith("Torch") ||
                                //itemDrop.gameObject.name.StartsWith("Arrow") ||
                                //itemDrop.gameObject.name.StartsWith("Fishing") ||
                                //itemDrop.gameObject.name.StartsWith("BombOoze"))
                            {
                                if (itemDrop.m_itemData.m_shared.m_attack != null)
                                {
                                    BetterStaminaPlugin.DebugLog($"Updating {itemDrop.gameObject.name} m_attackStamina from {itemDrop.m_itemData.m_shared.m_attack.m_attackStamina} to {BetterStaminaPlugin.bowAttackCost.Value}");
                                    itemDrop.m_itemData.m_shared.m_attack.m_attackStamina = BetterStaminaPlugin.bowAttackCost.Value;
                                }

//                                 if (itemDrop.m_itemData.m_shared.m_secondaryAttack != null)
//                                 {
//                                     string itemKey = itemDrop.gameObject.name + "_second";
//                                     if (!weaponStaminaStats.ContainsKey(itemKey))
//                                     {
//                                         BetterStaminaPlugin.DebugLog($"[ObjectDB] [2] Creating entry for {itemKey} ({itemDrop.m_itemData.m_shared.m_itemType}): Stamina - {itemDrop.m_itemData.m_shared.m_secondaryAttack.m_attackStamina}");
//                                         ConfigEntry<float> newItem = config.Bind("5.Weapons", itemKey, itemDrop.m_itemData.m_shared.m_secondaryAttack.m_attackStamina, "");
//                                         weaponStaminaStats.Add(itemKey, newItem);
//                                     }
//                                     else
//                                     {
//                                         BetterStaminaPlugin.DebugLog($"[ObjectDB] [2] Updating values for {itemKey} ({itemDrop.m_itemData.m_shared.m_itemType}): Stamina - {itemDrop.m_itemData.m_shared.m_secondaryAttack.m_attackStamina}");
//                                         itemDrop.m_itemData.m_shared.m_secondaryAttack.m_attackStamina = weaponStaminaStats[itemKey].Value;
//                                     }
//                                 }
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    [HarmonyPostfix]
    public static void ObjectDBAwake_PostFix(ObjectDB __instance)
    {
        UpdateWeaponStats(__instance);
    }
}
