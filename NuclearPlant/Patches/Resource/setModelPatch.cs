using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using UnityEngine;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(Resource), "setModel")]
    internal class setModelPatch
    {
        public static void Postfix(Resource __instance, GameObject model)
        {
            var resourceType = __instance.getResourceType();
            if(resourceType is UraniumOre
            || resourceType is UraniumRod)
                model.setColor(resourceType.getStatsColor());
        }
    }
}
