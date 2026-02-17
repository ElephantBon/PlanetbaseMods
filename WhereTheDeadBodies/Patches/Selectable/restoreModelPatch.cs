using HarmonyLib;
using Planetbase;
using System;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Restore corpse model on clear selection
    /// </summary>
    [HarmonyPatch(typeof(Selectable), nameof(Selectable.restoreModel))]
    internal class restoreModelPatch
    {
        public static bool Prefix(Selectable __instance)
        {
            if(__instance is Resource r && r.getResourceType() is Corpse) {             
                var mModel = __instance.getSelectionModel();
                if((Object)(object)mModel != (Object)null) {
                    mModel.restoreMaterialRecursive();
                    mModel.restoreMeshRecursive();
                }
                return false;
            }
            return true;
        }
    }
}
