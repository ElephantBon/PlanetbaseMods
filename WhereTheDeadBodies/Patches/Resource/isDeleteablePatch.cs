using HarmonyLib;
using Planetbase;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    [HarmonyPatch(typeof(Resource), nameof(Resource.isDeleteable))]
    internal class isDeleteablePatch
    {
        public static bool Prefix(Resource __instance, out bool __result, out bool buttonEnabled)
        {
            __result = buttonEnabled = false;
            if(__instance.getResourceType() is Corpse) {
                __result = buttonEnabled = false;
                return false;
            }
            return true;
        }
    }
}
