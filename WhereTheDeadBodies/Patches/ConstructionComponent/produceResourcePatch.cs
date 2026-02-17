using HarmonyLib;
using Planetbase;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), "produceResource")]
    internal class produceResourcePatch
    {
        public static bool Prefix(ConstructionComponent __instance, ResourceType resourceType, ResourceSubtype subtype, bool embedded)
        {
            if(__instance.getComponentType() is Incinerator)
                return false; // Do not create resource for production
            else
                return true;
        }
    }
}
