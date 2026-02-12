using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngineInternal;
using PlanetbaseModUtilities;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), nameof(ConstructionComponent.recycle))]
    internal class recyclePatch
    {
        public static bool Prefix(ConstructionComponent __instance)
        {
            if(!(__instance.getComponentType() is NuclearGenerator))
                return true;

            // Recycle generator will consume current producing uranium rod
            var mProductionProgress = CoreUtils.GetMember<ConstructionComponent, Indicator>("mProductionProgress", __instance);
            if(mProductionProgress == null || mProductionProgress.getValue() <= 0)
                return true;

            ResourceAmounts resourceAmounts = __instance.calculateRecycleAmounts();
            if(resourceAmounts == null) {
                return false;
            }

            int num = 0;
            foreach(ResourceAmount item in resourceAmounts) {
                for(int i = 0; i < item.getAmount() - 1; i++) {
                    Vector3 vector = new Vector3((float)(num % 2) - 0.5f, 0f, (float)(num / 2 % 2) - 0.5f);
                    Resource resource = Resource.create(item.getResourceType(), __instance.getPosition() + vector, Location.Interior);
                    resource.setRotation(__instance.getTransform().rotation);
                    resource.drop(Resource.State.Idle);
                    num++;
                }
            }

            return false;
        }
    }
}
