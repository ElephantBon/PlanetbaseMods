using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
