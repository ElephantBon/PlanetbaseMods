using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace FreeBuilding
{
    [HarmonyPatch(typeof(Module), nameof(Module.onBuilt))]
    public class onBuiltPatch
    {
        public static void Postfix(Module __instance)
        {
            // Prevent engineer stuck on building very short connections
            if(FreeBuilding.settings.InstantConnection)
                __instance.getLinks().ForEach(l => {
                     if(!l.isBuilt() && l is Connection)
                        l.onBuilt();
                });
        }
    }
}
