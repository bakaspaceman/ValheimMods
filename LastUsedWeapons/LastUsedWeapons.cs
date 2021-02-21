using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace LastUsedWeapons
{
    [BepInPlugin("bakaSpaceman.LastUsedWeapons", "Last Used Weapons", "1.0.0.0")]
    public class LastUsedWeapons : BaseUnityPlugin
    {
        private ConfigEntry<KeyboardShortcut> toggleLastEquippedShortcut;
        static private ConfigEntry<bool> ignoreTools;

        static MethodInfo _toggleEquipedMethod = typeof(Humanoid).GetMethod("ToggleEquiped", BindingFlags.Instance | BindingFlags.NonPublic);

        static FieldInfo _hiddenRightItem = typeof(Humanoid).GetField("m_hiddenRightItem", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _hiddenLeftItem = typeof(Humanoid).GetField("m_hiddenLeftItem", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _rightItem = typeof(Humanoid).GetField("m_rightItem", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _leftItem = typeof(Humanoid).GetField("m_leftItem", BindingFlags.Instance | BindingFlags.NonPublic);

        static ItemDrop.ItemData _lastRightItem = null;
        static ItemDrop.ItemData _lastLeftItem = null;
        static ItemDrop.ItemData _tempLastRightItem = null;
        static ItemDrop.ItemData _tempLastLeftItem = null;

        static bool _cacheLastitems = true;

        void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(LastUsedWeapons));

            toggleLastEquippedShortcut = Config.Bind("Key Bindings", "ToggleRecentWeapons", new KeyboardShortcut(KeyCode.Y), "Equips previous set of weapons");
            ignoreTools = Config.Bind("General", "IgnoreTools", false, "Will not remember tools as being last equipped (will preserve weapons held before them)");
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleLastEquippedShortcut.Value.MainKey))
            {
                ToggleLastEquippedItems();
            }
        }

        [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
        [HarmonyPrefix]
        static bool UnequipItem_Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects = true)
        {
            if (!_cacheLastitems)
            {
                return true;
            }

            if (item != null)
            {
                UnityEngine.Debug.Log(String.Format("UnequipItem_Prefix: item - {0}, type - {1}", item.m_shared != null ? item.m_shared.m_name : "", item.m_shared != null ? item.m_shared.m_itemType : 0));
            }

            bool bResult = CacheCurrentlyEquippedItems(__instance, ref _tempLastRightItem, ref _tempLastLeftItem);

            return bResult;
        }

        [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
        [HarmonyPostfix]
        static void UnequipItem_Postfix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects = true)
        {
            if (!_cacheLastitems)
            {
                //UnityEngine.Debug.Log("_cacheLastitems is false! early out");
                return;
            }

            if (ignoreTools.Value && item != null && item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool)
            {
                UnityEngine.Debug.Log("Ignoring tools due to config option!");
                return;
            }

            if (Player.m_localPlayer != __instance)
            {
                return;
            }

            bool bDidWork = false;
            if (_tempLastRightItem != (ItemDrop.ItemData)_rightItem.GetValue(__instance))
            {
                _lastRightItem = _tempLastRightItem;
                if (_lastRightItem != null)
                {
                    UnityEngine.Debug.Log(String.Format("UnequipItem_Postfix: last RIGHT set - {0}, type - {1}", _lastRightItem.m_shared.m_name, _lastRightItem.m_shared.m_itemType));
                }
                else
                {
                    UnityEngine.Debug.Log("UnequipItem_Postfix: las RIGHT set to empty.");
                }

                if (_lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon ||
                    _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool ||
                    _lastLeftItem != null && _lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
                {
                    _lastLeftItem = null;
                    UnityEngine.Debug.Log("UnequipItem_Postfix: last LEFT set to empty.");
                }

                bDidWork = true;
            }

            if (_tempLastLeftItem != (ItemDrop.ItemData)_leftItem.GetValue(__instance))
            {
                _lastLeftItem = _tempLastLeftItem;
                if (_lastLeftItem != null)
                {
                    UnityEngine.Debug.Log(String.Format("UnequipItem_Postfix: last LEFT set - {0}, type - {1}", _lastLeftItem.m_shared.m_name, _lastLeftItem.m_shared.m_itemType));
                }
                else
                {
                    UnityEngine.Debug.Log("UnequipItem_Postfix: last LEFT set to empty.");
                }

                if (_lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow ||
                    _lastRightItem != null && (_lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon || _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool))
                {
                    _lastRightItem = null;
                    UnityEngine.Debug.Log("UnequipItem_Postfix: last RIGHT set to empty.");
                }

                bDidWork = true;
            }

            _tempLastRightItem = null;
            _tempLastLeftItem = null;

            if (bDidWork)
            {
                UnityEngine.Debug.Log("");
            }
        }

        [HarmonyPatch(typeof(Humanoid), "HideHandItems")]
        [HarmonyPrefix]
        static bool HideHandItems_Prefix()
        {
            _cacheLastitems = false;
            return true;
        }

        [HarmonyPatch(typeof(Humanoid), "HideHandItems")]
        [HarmonyPostfix]
        static void HideHandItems_Postfix()
        {
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
            if ((ItemDrop.ItemData)_hiddenLeftItem.GetValue(Player.m_localPlayer) != null ||
                (ItemDrop.ItemData)_hiddenRightItem.GetValue(Player.m_localPlayer) != null)
            {
                if (!Player.m_localPlayer.IsSwiming() || Player.m_localPlayer.IsOnGround())
                {
                    UnityEngine.Debug.Log("");
                    UnityEngine.Debug.Log("--- [Toggle Last Equipped] START ---");
                    Player.m_localPlayer.ShowHandItems();
                    UnityEngine.Debug.Log("ShowHandItems() instead");
                    UnityEngine.Debug.Log("");
                }

                return;
            }

            UnityEngine.Debug.Log("");
            UnityEngine.Debug.Log("--- [Toggle Last Equipped] START ---");

            _cacheLastitems = false;
            ItemDrop.ItemData tempLastRightItem = null;
            ItemDrop.ItemData tempLastLeftItem = null;
            CacheCurrentlyEquippedItems(Player.m_localPlayer, ref tempLastRightItem, ref tempLastLeftItem);

            bool equippedRightItem = false;
            bool equippedLeftItem = false;

            bool cacheLeftArm = tempLastLeftItem != null && _lastRightItem != null && _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool;

            if (_lastRightItem != null)
            {
                if (_lastLeftItem != null && _lastLeftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch)
                {
                    _lastLeftItem = null;
                    UnityEngine.Debug.Log("ToggleLastEquippedItems: last LEFT set to empty because its a torch.");
                }

                UnityEngine.Debug.Log(String.Format("Trying to equip {0}({1}) to right arm", _lastRightItem.m_shared.m_name, _lastRightItem.m_shared.m_itemType));

                bool bNewItemIsTool = _lastRightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool;
                equippedRightItem = (bool)_toggleEquipedMethod.Invoke(Player.m_localPlayer, new object[] { _lastRightItem });
                if (equippedRightItem)
                {
                    if (!ignoreTools.Value || tempLastRightItem == null || tempLastRightItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Tool)
                    {
                        _lastRightItem = tempLastRightItem;

                        if (_lastRightItem != null)
                        {
                            UnityEngine.Debug.Log(String.Format("ToggleLastEquippedItems: last RIGHT set - {0}, type - {1}", _lastRightItem.m_shared.m_name, _lastRightItem.m_shared.m_itemType));
                        }
                        else
                        {
                            UnityEngine.Debug.Log("ToggleLastEquippedItems: last RIGHT set to empty.");
                        }
                    }
                    else if (ignoreTools.Value && !bNewItemIsTool)
                    {
                        _lastRightItem = null;
                        UnityEngine.Debug.Log("ToggleLastEquippedItems: last RIGHT set to empty.");
                    }
                }
            }

            if (_lastLeftItem != null)
            {
                UnityEngine.Debug.Log(String.Format("Trying to equip {0}({1}) to left arm", _lastLeftItem.m_shared.m_name, _lastLeftItem.m_shared.m_itemType));

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
                    UnityEngine.Debug.Log("Successful");

                    if (!ignoreTools.Value || tempLastLeftItem == null || tempLastLeftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Tool)
                    {
                        _lastLeftItem = tempLastLeftItem;

                        if (_lastLeftItem != null)
                        {
                            UnityEngine.Debug.Log(String.Format("ToggleLastEquippedItems: last LEFT set - {0}, type - {1}", _lastLeftItem.m_shared.m_name, _lastLeftItem.m_shared.m_itemType));
                        }
                        else
                        {
                            UnityEngine.Debug.Log("ToggleLastEquippedItems: last LEFT set to empty.");
                        }
                    }
                    else if (ignoreTools.Value && !bNewItemIsTool)
                    {
                        _lastLeftItem = null;
                        UnityEngine.Debug.Log("ToggleLastEquippedItems: last LEFT set to empty.");
                    }
                }
            }
            else if (cacheLeftArm)
            {
                _lastLeftItem = tempLastLeftItem;
            }

            _cacheLastitems = true;

            UnityEngine.Debug.Log("--- [Toggle Last Equipped] END ---");
            UnityEngine.Debug.Log("");
        }

        private static bool CacheCurrentlyEquippedItems(Humanoid __instance, ref ItemDrop.ItemData lastRightItem, ref ItemDrop.ItemData lastLeftItem)
        {
            if (Player.m_localPlayer != __instance || __instance == null)
            {
                return true;
            }

            if (_rightItem != null)
            {
                lastRightItem = (ItemDrop.ItemData)_rightItem.GetValue(__instance);
                if (lastRightItem != null)
                {
                    UnityEngine.Debug.Log(String.Format("Caching currently equipped RIGHT item - {0}, type - {1}", lastRightItem.m_shared != null ? lastRightItem.m_shared.m_name : "", lastRightItem.m_shared != null ? lastRightItem.m_shared.m_itemType : 0));
                }
            }

            if (_leftItem != null)
            {
                lastLeftItem = (ItemDrop.ItemData)_leftItem.GetValue(__instance);
                if (lastLeftItem != null)
                {
                    UnityEngine.Debug.Log(String.Format("Caching currently equipped LEFT item - {0}, type - {1}", lastLeftItem.m_shared != null ? lastLeftItem.m_shared.m_name : "", lastLeftItem.m_shared != null ? lastLeftItem.m_shared.m_itemType : 0));
                }
            }

            return true;
        }
    }
}
