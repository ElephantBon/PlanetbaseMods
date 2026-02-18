using HarmonyLib;
using Planetbase;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Remove output of Incinerator
    /// </summary>
    [HarmonyPatch(typeof(ConstructionComponent), "produceResource")]
    internal class produceResourcePatch
    {
        public static bool Prefix(ConstructionComponent __instance, ResourceType resourceType, ResourceSubtype subtype, bool embedded)
        {
            if(__instance.getComponentType() is Incinerator)
                return false;
            else
                return true;
        }
    }
}
