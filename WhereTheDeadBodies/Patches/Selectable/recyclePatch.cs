using HarmonyLib;
using Planetbase;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Dead human creates resource Corpse
    /// </summary>
    [HarmonyPatch(typeof(Selectable), "recycle")]
    internal class recyclePatch
    {
        public static void Postfix(Selectable __instance)
        {
            if(__instance is Human human) { 
                Corpse.CreateResource(human);
            }
        }
    }
}
