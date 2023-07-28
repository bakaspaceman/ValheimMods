using BepInEx;
using HarmonyLib;

namespace PlayerMapBlipToggle
{
  [BepInPlugin("bakaSpaceman.playermapbliptoggle", "Player Map Blip Toggle", "2.0.0")]
    public class PlayerMapBlipToggle : BaseUnityPlugin
    {
        void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(PlayerMapBlipToggle));
        }

        [HarmonyPatch(typeof(Minimap),"Awake")]
        [HarmonyPostfix]
        static void MinimapAwake_Postfix(Minimap __instance)
        {
            __instance.m_publicPosition.isOn = true;
            ZNet.instance.SetPublicReferencePosition(__instance.m_publicPosition.isOn);
        }
    }
}
