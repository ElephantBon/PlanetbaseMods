using HarmonyLib;
using Planetbase;
using UnityEngine;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    [HarmonyPatch(typeof(Module), nameof(Module.onBuilt))]
    internal class onBuiltPatch
    {
        public static void Postfix(Module __instance)
        {
            if(__instance.getModuleType() is ModuleTypeMorgue) { 
                var blood = GameObject.Instantiate(ContentManager.PrefabBloodSplatter);
                blood.transform.parent = __instance.getTransform();
                blood.transform.localPosition = new Vector3(0, 0.25f, 0);
                blood.transform.localScale = new Vector3(7f, 7f, 7f);
            }
        }
    }
}
