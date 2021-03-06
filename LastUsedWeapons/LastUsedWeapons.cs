using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace LastUsedWeapons
{
    [BepInPlugin("bakaSpaceman.LastUsedWeapons", "Last Used Weapons", "1.1.0.0")]
    public class LastUsedWeapons : BepInPluginTemplate
    {
        // Config - General
        static public ConfigEntry<int> nexusID;
        private ConfigEntry<KeyboardShortcut> toggleLastEquippedShortcut;
        static public ConfigEntry<bool> autoEquipAfterSwimming;

        static MethodInfo _toggleEquipedMethod = typeof(Humanoid).GetMethod("ToggleEquiped", BindingFlags.Instance | BindingFlags.NonPublic);
        static MethodInfo _takeInputMethod = typeof(Player).GetMethod("TakeInput", BindingFlags.Instance | BindingFlags.NonPublic);

        static FieldInfo _hiddenRightItemField = typeof(Humanoid).GetField("m_hiddenRightItem", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _hiddenLeftItemField = typeof(Humanoid).GetField("m_hiddenLeftItem", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _rightItemField = typeof(Humanoid).GetField("m_rightItem", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _leftItemField = typeof(Humanoid).GetField("m_leftItem", BindingFlags.Instance | BindingFlags.NonPublic);

        static ItemDrop.ItemData _lastRightItem = null;
        static ItemDrop.ItemData _lastLeftItem = null;
        static ItemDrop.ItemData _equippedRightItem = null;
        static ItemDrop.ItemData _equippedLeftItem = null;

        static bool _cacheLastitems = true;
        static bool _hiddenItemsForSwimming = false;

        private void SetupConfig()
        {
            nexusID = Config.Bind("General", "NexusID", 259, "Nexus mod ID for updates");
            toggleLastEquippedShortcut = Config.Bind("Key Bindings", "ToggleLastUsedWeapons", new KeyboardShortcut(KeyCode.T), "Equips previous set of weapons");
            autoEquipAfterSwimming = Config.Bind("General", "AutoEquipOnLeavingWater", true, "Will automatically restore weapons that were unequipped on entering water");
        }

        protected override void Awake()
        {
            base.Awake();

            SetupConfig();

            harmonyInst.PatchAll(typeof(LastUsedWeapons));
        }

        void Update()
        {
            if (Player.m_localPlayer != null)
            {
                bool acceptInput = (bool)_takeInputMethod.Invoke(Player.m_localPlayer, new object[] {});
                if (acceptInput && Input.GetKeyDown(toggleLastEquippedShortcut.Value.MainKey))
                {
                    ToggleLastEquippedItems();
                }

                if (autoEquipAfterSwimming.Value && (!Player.m_localPlayer.IsSwiming() || Player.m_localPlayer.IsOnGround()))
                {
                    if (_hiddenItemsForSwimming)
                    {
                        Player.m_localPlayer.ShowHandItems();
                    }

                    _hiddenItemsForSwimming = false;
                }
            }
        }

        private static void SetLastRightItem(ItemDrop.ItemData newRightItem)
        {
            string callingMethodName = new StackFrame(1).GetMethod().Name;

            _lastRightItem = newRightItem;
            if (_lastRightItem != null)
            {
                DebugLog($"SetLastRightItem: ({callingMethodName}) set - {_lastRightItem.m_shared.m_name}, type - {_lastRightItem.m_shared.m_itemType}");
            }
            else
            {
                DebugLog($"SetLastRightItem: ({callingMethodName}) set to empty.");
            }

            if (_lastRightItem != null && _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon ||
                _lastRightItem != null && _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool ||
                _lastLeftItem != null && _lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
            {
                _lastLeftItem = null;
                DebugLog($"SetLastRightItem: ({callingMethodName}) last LEFT set to empty.");
            }
        }

        private static void SetLastLeftItem(ItemDrop.ItemData newLeftItem)
        {
            string callingMethodName = new StackFrame(1).GetMethod().Name;

            _lastLeftItem = newLeftItem;
            if (_lastLeftItem != null)
            {
                DebugLog($"SetLastLeftItem: ({callingMethodName}) set - {_lastLeftItem.m_shared.m_name}, type - {_lastLeftItem.m_shared.m_itemType}");
            }
            else
            {
                DebugLog($"SetLastLeftItem: ({callingMethodName}) set to empty.");
            }

            if (_lastLeftItem != null && _lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow ||
                _lastRightItem != null && (_lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon || _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool))
            {
                _lastRightItem = null;
                DebugLog($"SetLastLeftItem: ({callingMethodName}) last RIGHT set to empty.");
            }
        }

        [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
        [HarmonyPrefix]
        static bool UnequipItem_Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects = true)
        {
            if (__instance != Player.m_localPlayer)
                return true;

            if (!_cacheLastitems)
            {
                return true;
            }

            if (item != null && item.m_shared != null)
            {
                DebugLog($"UnequipItem_Prefix: item - {item.m_shared.m_name}, type - {item.m_shared.m_itemType}");
            }
            else
            {
                return true;
            }

            bool bResult = CacheCurrentlyEquippedItems(__instance, ref _equippedRightItem, ref _equippedLeftItem);

            return bResult;
        }

        [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
        [HarmonyPostfix]
        static void UnequipItem_Postfix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects = true)
        {
            if (!_cacheLastitems)
            {
                return;
            }

            if (item != null && item.m_shared != null)
            {
                DebugLog($"UnequipItem_Postfix: item - {item.m_shared.m_name}, type - {item.m_shared.m_itemType}");
            }
            else
            {
                return;
            }

            if (Player.m_localPlayer != __instance)
            {
                return;
            }

            if (_equippedRightItem != (ItemDrop.ItemData)_rightItemField.GetValue(__instance))
            {
                SetLastRightItem(_equippedRightItem);
            }

            if (_equippedLeftItem != (ItemDrop.ItemData)_leftItemField.GetValue(__instance))
            {
                SetLastLeftItem(_equippedLeftItem);
            }

            _equippedRightItem = null;
            _equippedLeftItem = null;
        }

        [HarmonyPatch(typeof(Humanoid), "EquipItem")]
        [HarmonyPrefix]
        static bool EquipItem_Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects = true)
        {
            if (__instance != Player.m_localPlayer)
                return true;

            DebugLog("");
            if (item != null && item.m_shared != null)
            {
                DebugLog($"EquipItem_Prefix: item - {item.m_shared.m_name}, type - {item.m_shared.m_itemType}");
            }
            else
            {
                DebugLog($"EquipItem_Prefix: item - NULL");
                return true;
            }

            if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool ||
                item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch ||
                item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon ||
                item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield ||
                item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow ||
                item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon)
            {
                ItemDrop.ItemData hiddenLeftItem = (ItemDrop.ItemData)_hiddenLeftItemField.GetValue(__instance);
                ItemDrop.ItemData hiddenRightItem = (ItemDrop.ItemData)_hiddenRightItemField.GetValue(__instance);

                if (hiddenRightItem != null)
                {
                    SetLastRightItem(hiddenRightItem);
                }

                if (hiddenLeftItem != null)
                {
                    SetLastLeftItem(hiddenLeftItem);
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(Humanoid), "HideHandItems")]
        [HarmonyPrefix]
        static bool HideHandItems_Prefix(Humanoid __instance)
        {
            if (__instance != Player.m_localPlayer)
                return true;

            if (__instance.IsSwiming() && !__instance.IsOnGround())
            {
                if ((ItemDrop.ItemData)_rightItemField.GetValue(__instance) != null ||
                    (ItemDrop.ItemData)_leftItemField.GetValue(__instance) != null)
                {
                    _hiddenItemsForSwimming = true;
                }
            }

            _cacheLastitems = false;
            return true;
        }

        [HarmonyPatch(typeof(Humanoid), "HideHandItems")]
        [HarmonyPostfix]
        static void HideHandItems_Postfix(Humanoid __instance)
        {
            if (__instance != Player.m_localPlayer)
                return;

            _cacheLastitems = true;
        }

        void ToggleLastEquippedItems()
        {
            if (Player.m_localPlayer == null)
            {
                return;
            }

            if (Player.m_localPlayer.IsRunning())
            {
                return;
            }

            // If player used "R" to hide weapons, then make this mod just work like hitting "R" again and unholster them
            if ((ItemDrop.ItemData)_hiddenLeftItemField.GetValue(Player.m_localPlayer) != null ||
                (ItemDrop.ItemData)_hiddenRightItemField.GetValue(Player.m_localPlayer) != null)
            {
                if (!Player.m_localPlayer.IsSwiming() || Player.m_localPlayer.IsOnGround())
                {
                    DebugLog("");
                    DebugLog("--- [Toggle Last Equipped] START ---");
                    Player.m_localPlayer.ShowHandItems();
                    DebugLog("ShowHandItems() instead");
                    DebugLog("");
                }

                return;
            }

            DebugLog("");
            DebugLog("--- [Toggle Last Equipped] START ---");

            _cacheLastitems = false;
            ItemDrop.ItemData tempLastRightItem = null;
            ItemDrop.ItemData tempLastLeftItem = null;
            CacheCurrentlyEquippedItems(Player.m_localPlayer, ref tempLastRightItem, ref tempLastLeftItem);

            bool equippedRightItem = false;
            bool equippedLeftItem = false;

            bool cacheLeftArm = tempLastLeftItem != null && _lastRightItem != null && _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool;

            if (_lastRightItem != null)
            {
                if(tempLastRightItem != null && _lastRightItem.m_shared.m_name == tempLastRightItem.m_shared.m_name)
                {
                    _lastRightItem = null;
                    DebugLog("ToggleLastEquippedItems: last RIGHT item is same as current equipped one. Setting to empty.");
                }
                else
                {
                    if (_lastLeftItem != null && _lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch)
                    {
                        _lastLeftItem = null;
                        DebugLog("ToggleLastEquippedItems: last LEFT set to empty because its a torch.");
                    }

                    DebugLog($"Trying to equip {_lastRightItem.m_shared.m_name}({_lastRightItem.m_shared.m_itemType}) to right arm");

                    bool bNewItemIsTool = _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool;
                    equippedRightItem = (bool)_toggleEquipedMethod.Invoke(Player.m_localPlayer, new object[] { _lastRightItem });
                    if (equippedRightItem)
                    {
                        DebugLog("Successful");

                        _lastRightItem = tempLastRightItem;

                        if (_lastRightItem != null)
                        {
                            DebugLog($"ToggleLastEquippedItems: last RIGHT set - {_lastRightItem.m_shared.m_name}, type - {_lastRightItem.m_shared.m_itemType}");
                        }
                        else
                        {
                            DebugLog("ToggleLastEquippedItems: last RIGHT set to empty.");
                        }
                    }
                }
            }

            if (_lastLeftItem != null)
            {
                if (tempLastLeftItem != null && _lastLeftItem.m_shared.m_name == tempLastLeftItem.m_shared.m_name)
                {
                    _lastLeftItem = null;
                    DebugLog("ToggleLastEquippedItems: last LEFT item is same as current equipped one. Setting to empty.");
                }
                else
                {
                    DebugLog($"Trying to equip {_lastLeftItem.m_shared.m_name}({_lastLeftItem.m_shared.m_itemType}) to left arm");

                    bool bNewItemIsTool = _lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool;
                    if (equippedRightItem)
                    {
                        equippedLeftItem = Player.m_localPlayer.EquipItem(_lastLeftItem);
                    }
                    else
                    {
                        equippedLeftItem = (bool)_toggleEquipedMethod.Invoke(Player.m_localPlayer, new object[] { _lastLeftItem });
                    }

                    if (equippedLeftItem)
                    {
                        DebugLog("Successful");

                        _lastLeftItem = tempLastLeftItem;

                        if (_lastLeftItem != null)
                        {
                            DebugLog($"ToggleLastEquippedItems: last LEFT set - {_lastLeftItem.m_shared.m_name}, type - {_lastLeftItem.m_shared.m_itemType}");
                        }
                        else
                        {
                            DebugLog("ToggleLastEquippedItems: last LEFT set to empty.");
                        }
                    }
                }
            }
            else if (cacheLeftArm)
            {
                _lastLeftItem = tempLastLeftItem;
            }

            _cacheLastitems = true;

            DebugLog("--- [Toggle Last Equipped] END ---");
            DebugLog("");
        }

        private static bool CacheCurrentlyEquippedItems(Humanoid __instance, ref ItemDrop.ItemData rightItem, ref ItemDrop.ItemData leftItem)
        {
            if (Player.m_localPlayer != __instance || __instance == null)
            {
                return true;
            }

            if (_rightItemField != null)
            {
                rightItem = (ItemDrop.ItemData)_rightItemField.GetValue(__instance);
                if (rightItem != null && rightItem.m_shared != null)
                {
                    DebugLog($"Currently equipped RIGHT item - {rightItem.m_shared.m_name}, type - {rightItem.m_shared.m_itemType}");
                }
            }

            if (_leftItemField != null)
            {
                leftItem = (ItemDrop.ItemData)_leftItemField.GetValue(__instance);
                if (leftItem != null && leftItem.m_shared != null)
                {
                    DebugLog($"Currently equipped LEFT item - {leftItem.m_shared.m_name}, type - {leftItem.m_shared.m_itemType}");
                }
            }

            return true;
        }
    }
}
