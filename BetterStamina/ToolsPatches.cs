using BetterStamina;
using HarmonyLib;
using System.Diagnostics;

internal static class ToolsPatches
{
    [HarmonyPatch(typeof(Player), "UseStamina")]
    [HarmonyPrefix]
    private static bool HammerStamina_Prefix(Player __instance, ref float v)
    {
        if (__instance.GetRightItem() != null)
        {
            string name = __instance.GetRightItem().m_shared.m_name;
            string callingMethodName = new StackFrame(2).GetMethod().Name;

            if (callingMethodName.Contains("Repair") || callingMethodName.Contains("UpdatePlacement"))
            {
                if (name == "$item_hammer" && BetterStaminaPlugin.removeHammerStaminaCost.Value ||
                    name == "$item_hoe" && BetterStaminaPlugin.removeHoeStaminaCost.Value ||
                    name == "$item_cultivator" && BetterStaminaPlugin.removeCultivatorStaminaCost.Value)
                {
                    v = 0.0f;
                }
            }
        }

        return true;
    }
}