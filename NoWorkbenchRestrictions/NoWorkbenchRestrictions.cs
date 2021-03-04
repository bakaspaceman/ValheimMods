
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace NoWorkbenchRestrictions
{
    [BepInPlugin("bakaSpaceman.NoWorkbenchRestrictions", "No Workbench Restrictions", "1.0.0.0")]
    public class NoWorkbenchRestrictionsMod : BepInPluginTemplate
    {
        private static Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>();

        //Config - General
        static public ConfigEntry<bool> hideStationInRecipies;

        private void SetupConfig()
        {
            //nexusID = Config.Bind("General", "NexusID", 259, "Nexus mod ID for updates");
            hideStationInRecipies = Config.Bind("General", "HideStationsInRecipeRequirements", true, "Will hide station icon in the recipe requirements UI");
        }

        protected override void Awake()
        {
            base.Awake();

            SetupConfig();

            harmonyInst.PatchAll(typeof(NoWorkbenchRestrictionsMod));
        }

        [HarmonyPatch(typeof(Player), "HaveRequirements", new Type[] { typeof(Piece), typeof(Player.RequirementMode) })]
        [HarmonyPostfix]
        static public void HaveRequirements_Postfix(Piece piece, Player.RequirementMode mode, bool __result)
        {
            DebugLog($"Player.HaveRequirements_Postfix(): result - {__result}; piece - {piece.m_name}; mode - {mode}");
        }
        
        [HarmonyPatch(typeof(Inventory), "CountItems")]
        [HarmonyPostfix]
        static public void CountItems_Postfix(String name, int __result)
        {
            //DebugLog($"Inventory.CountItems_Postfix(): result - {__result}; name - {name}");
        }

        [HarmonyPatch(typeof(Hud), "SetupPieceInfo")]
        [HarmonyPostfix]
        static public void SetupPieceInfo_Postfix(Piece piece, GameObject[] ___m_requirementItems)
        {
            DebugLog($"Hud.SetupPieceInfo(): name - {piece.m_name}");

            GameObject requirementItem = ___m_requirementItems[piece.m_resources.Length];

            if (hideStationInRecipies.Value)
            {
                requirementItem.SetActive(false);
                return;
            }

            Image component1 = requirementItem.transform.Find("res_icon").GetComponent<Image>();
            Text component3 = requirementItem.transform.Find("res_amount").GetComponent<Text>();

            component1.color = Color.white;
            component3.text = "";
            component3.color = Color.white;
        }

        static public bool HaveBuildStationsInRange(string name, Vector3 point)
        {
            DebugLog($"NoWorkbenchRestrictionsMod.HaveBuildStationsInRange(): true; name - {name}; point - {point.ToString()}");
            return true;
        }

        [HarmonyPatch(typeof(Player), "CheckCanRemovePiece")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CheckCanRemovePiece_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            DebugTranspilerLog($"######## CheckCanRemovePiece_Transpiler START ########");
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                CodeInstruction instr = codes[i];

                DebugTranspilerLog($"{i} {instr}");

                if (instr.opcode == OpCodes.Call)
                {
                    String instrString = instr.ToString();
                    if (instrString.Contains("HaveBuildStationInRange"))
                    {
                        int foundImplicitIndex = -1;
                        int foundBrIndex = -1;
                        for (var j = i + 1; j < i + 5; j++)           // UnityEngine.Object::op_Implicit & brtrue instruction needs to be within next 5
                        {
                            DebugTranspilerLog($"v{j} {codes[j].ToString()}");

                            if (codes[j].opcode == OpCodes.Brtrue_S || codes[j].opcode == OpCodes.Brtrue)
                            {
                                foundBrIndex = j;
                            }
                            if (codes[j].opcode == OpCodes.Call)
                            {
                                instrString = codes[j].ToString();
                                if (instrString.Contains("UnityEngine.Object::op_Implicit"))
                                {
                                    foundImplicitIndex = j;
                                }
                            }
                        }

                        if (foundBrIndex != -1 && foundImplicitIndex != -1)
                        {
                            DebugTranspilerLog($">>> Deleting instruction: {foundImplicitIndex} {codes[foundImplicitIndex].ToString()}:");
                            codes.RemoveAt(foundImplicitIndex);

                            int insertIndex = i;
                            DebugTranspilerLog($">>> Deleting instruction: {insertIndex} {codes[insertIndex].ToString()}:");
                            codes.RemoveAt(insertIndex);

                            DebugTranspilerLog($">>> Inserting instruction at {insertIndex}:");
                            DebugTranspilerLog($"Old: { codes[insertIndex].ToString()}");
                            codes.Insert(insertIndex, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NoWorkbenchRestrictionsMod), "HaveBuildStationsInRange")));
                            DebugTranspilerLog($"New: { codes[insertIndex].ToString()}");
                            break;
                        }
                    }
                }
            }

            PrintOutInstructions(codes);

            DebugTranspilerLog($"######## CheckCanRemovePiece_Transpiler END ########");
            DebugTranspilerLog($"");

            return codes;
        }

        [HarmonyPatch(typeof(Player), "HaveRequirements", new Type[] { typeof(Piece),typeof(Player.RequirementMode) })]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> HaveRequirements_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            DebugTranspilerLog($"######## HaveRequirements_Transpiler START ########");
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                CodeInstruction instr = codes[i];

                DebugTranspilerLog($"{i} {instr}");

                if (instr.opcode == OpCodes.Call)
                {
                    String instrString = instr.ToString();
                    if (instrString.Contains("HaveBuildStationInRange"))
                    {
                        int foundImplicitIndex = -1;
                        int foundBrIndex = -1;
                        for (var j = i + 1; j < i + 5; j++)           // UnityEngine.Object::op_Implicit & brtrue instruction needs to be within next 5
                        {
                            DebugTranspilerLog($"v{j} {codes[j].ToString()}");

                            if (codes[j].opcode == OpCodes.Brtrue_S || codes[j].opcode == OpCodes.Brtrue)
                            {
                                foundBrIndex = j;
                            }
                            if (codes[j].opcode == OpCodes.Call)
                            {
                                instrString = codes[j].ToString();
                                if (instrString.Contains("UnityEngine.Object::op_Implicit"))
                                {
                                    foundImplicitIndex = j;
                                }
                            }
                        }

                        if (foundBrIndex != -1 && foundImplicitIndex != -1)
                        {
                            DebugTranspilerLog($">>> Deleting instruction: {foundImplicitIndex} {codes[foundImplicitIndex].ToString()}:");
                            codes.RemoveAt(foundImplicitIndex);

                            int insertIndex = i;
                            DebugTranspilerLog($">>> Deleting instruction: {insertIndex} {codes[insertIndex].ToString()}:");
                            codes.RemoveAt(insertIndex);

                            DebugTranspilerLog($">>> Inserting instruction at {insertIndex}:");
                            DebugTranspilerLog($"Old: { codes[insertIndex].ToString()}");
                            codes.Insert(insertIndex, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NoWorkbenchRestrictionsMod), "HaveBuildStationsInRange")));
                            DebugTranspilerLog($"New: { codes[insertIndex].ToString()}");
                            break;
                        }
                    }
                }
            }

            PrintOutInstructions(codes);

            DebugTranspilerLog($"######## HaveRequirements_Transpiler END ########");
            DebugTranspilerLog($"");

            return codes;
        }
    }
}
