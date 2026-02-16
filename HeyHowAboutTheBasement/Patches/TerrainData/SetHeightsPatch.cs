using HarmonyLib;
using UnityEngine;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(TerrainData), "SetHeights")]
    internal class SetHeightsPatch
    {
        public static bool Prefix()
        {
            if(HeyHowAboutTheBasement.creatingConnectionTunnelEntrance)
                return false;
            else
                return true;
        }
    }
}
