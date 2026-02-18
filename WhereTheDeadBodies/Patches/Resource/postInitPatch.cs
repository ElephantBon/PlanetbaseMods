using HarmonyLib;
using Planetbase;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Because corpse states are not stored in save file, replace resources with random corpse 3D on loading a save
    /// </summary>
    [HarmonyPatch(typeof(Resource), "postInit")]
    internal class postInitPatch
    {
        public static void Postfix(Resource __instance)
        {
            if(__instance.getResourceType() is Corpse)
                Corpse.ReplaceVisualRandom(__instance);
        }
    }
}
