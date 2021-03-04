using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BuildEverything
{
    [BepInPlugin("bakaSpaceman.BuildEverything", "Build Everything", "1.0.0.0")]
    public class BuildEverythingMod : BepInPluginTemplate
    {
        private static Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>();

        private void SetupConfig()
        {
            //nexusID = Config.Bind("General", "NexusID", 259, "Nexus mod ID for updates");
        }

        protected override void Awake()
        {
            base.Awake();

            SetupConfig();

            harmonyInst.PatchAll(typeof(BuildEverythingMod));
        }

        protected override void OnDestroy()
        {
            DebugLog($"BuildEverythingMod.OnDestroy()");
            foreach (KeyValuePair<string, GameObject> entry in prefabList)
            {
                UnityEngine.Object.Destroy((GameObject)entry.Value);
            }

            prefabList.Clear();

            base.OnDestroy();
        }

        [HarmonyPatch(typeof(CraftingStation), "HaveBuildStationInRange")]
        [HarmonyPrefix]
        public static bool HaveBuildStationInRange_Prefix(string name, Vector3 point, ref CraftingStation __result)
        {
            // 'name' format:
            //"$piece_workbench"
            //"$piece_stonecutter"
            //"$piece_forge"

            if (Player.m_localPlayer == null)
                return true;

            if (!prefabList.ContainsKey(name))
            {
                GameObject stationPrefab = null;
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    Piece pieceComp = go.GetComponent<Piece>();
                    if (pieceComp != null)
                    {
                        if (pieceComp.m_name == name)
                        {
                            stationPrefab = go;
                            break;
                        }
                    }
                }

                GameObject newStationObject = null;
                if (stationPrefab != null)
                {
                    DebugLog($"Instantiating {name}");
                    newStationObject = UnityEngine.Object.Instantiate<GameObject>(stationPrefab, point, Quaternion.identity);
                    if (newStationObject != null)
                    {
                        Piece component1 = newStationObject.GetComponent<Piece>();
                        if ((bool)(UnityEngine.Object)component1)
                        {
                            component1.SetCreator(Player.m_localPlayer.GetPlayerID());
                        }
                        PrivateArea component2 = newStationObject.GetComponent<PrivateArea>();
                        if ((bool)(UnityEngine.Object)component2)
                        {
                            component2.Setup(Game.instance.GetPlayerProfile().GetName());
                        }
                        WearNTear component3 = newStationObject.GetComponent<WearNTear>();
                        if ((bool)(UnityEngine.Object)component3)
                        {
                            component3.OnPlaced();
                        }
                    }
                }

                prefabList.Add(name, newStationObject);
            }

            DebugLog($"Searching existing entries:");
            foreach (KeyValuePair<string, GameObject> entry in prefabList)
            {
                CraftingStation dum = null;
                if (entry.Value != null)
                {
                    dum = entry.Value.GetComponent<CraftingStation>();
                }

                DebugLog($"[{entry.Key}]: found GO - {entry.Value != null}; CS - {dum != null}");
            }

            CraftingStation dummyStation = null;
            GameObject existingPrefab = prefabList[name];
            if (existingPrefab != null)
            {
                dummyStation = existingPrefab.GetComponent<CraftingStation>();
            }

            if (dummyStation != null)
            {
                DebugLog($"Successfully found dummy crafting station for {name}");
                __result = dummyStation;
                return false;
            }
            else
            {
                DebugLog($"Failed to find dummy crafting station for {name}");
                return true;
            }
        }
    }
}
