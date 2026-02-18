using HarmonyLib;
using Planetbase;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Show selection outline for corpse
    /// </summary>
    [HarmonyPatch(typeof(Selectable), nameof(Selectable.useExtrudedSelection))]
    internal class useExtrudedSelectionPatch
    {
        public static bool Prefix(Selectable __instance, ref bool __result)
        {
            if(__instance is Resource r && r.getResourceType() is Corpse) {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
