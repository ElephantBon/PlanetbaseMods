using HarmonyLib;
using HeyHowAboutTheBasement.Models;
using Planetbase;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Module), nameof(Module.destroy))]
    internal class destroyPatch
    {
        public static bool Prefix(Module __instance)
        {
            // Remove controller in linked module if exist
            var m2 = WalkwayModel.GetLinked(__instance);
            if(m2 != null) 
                WalkwayModel.DestroyWalkwayController(m2);
            return true;
        }
    }
}
