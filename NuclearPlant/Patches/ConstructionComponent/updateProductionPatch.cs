using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), "updateProduction")]
    internal class updateProductionPatch
    {
        public static bool Prefix(ConstructionComponent __instance, float timeStep)
        {
            // Operator doesn't necessary for nuclear generator 

            var mComponentType = __instance.getComponentType();
            if(!(mComponentType is NuclearGenerator))
                return true; // original
                        
            bool isEnabled = enabled(__instance);


            // Switch component type idle/powered
            var newComponentType = (isEnabled ?
                ComponentTypeList.find<NuclearGeneratorPowered>() :
                ComponentTypeList.find<NuclearGeneratorIdle>());
            if(__instance.getComponentType() != newComponentType) {
                CoreUtils.SetMember("mComponentType", __instance, newComponentType);
                CoreUtils.InvokeMethod("updateDescriptionItems", __instance);
            }

            if(!isEnabled) 
                return false;

            // Consume resource
            float num = 1f;
            float resourceProductionPeriod = mComponentType.getResourceProductionPeriod();
            var mProductionProgress = CoreUtils.GetMember<ConstructionComponent, Indicator>("mProductionProgress", __instance);
            if(mProductionProgress.increase(num * timeStep / resourceProductionPeriod)) {
                var mResourceContainer = __instance.getResourceContainer();
                consumeResources(mComponentType.getResourceConsumption(), mResourceContainer);
                mProductionProgress.setValue(0f);
            }

            // Update maintenance level
            var constructionPlant = __instance.getParentConstruction();
            var modulePlant = constructionPlant as Module;
            var mConditionIndicator = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", constructionPlant);
            var updateValue = timeStep / (modulePlant.getModuleType().getConditionDecayTime(modulePlant.getSizeIndex()));

            if(modulePlant.hasWater()) {
                if(__instance.anyInteractions())
                    mConditionIndicator.increase(updateValue);
                else
                    mConditionIndicator.decrease(updateValue);
            }
            else {
                // Decrease more on water shortage
                mConditionIndicator.decrease(updateValue * 5);
            }

            if(mConditionIndicator.getValue() <= 0.1f)  // Don't set too low or the module will disable
                ModuleTypeNuclearPlant.Explode(modulePlant);


            return false;
        }

        private static bool enabled(ConstructionComponent __instance)
        {
            var mParentConstruction = __instance.getParentConstruction();
            if(!mParentConstruction.isEnabled())
                return false;

            var mConditionIndicator = CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", __instance);
            if(mConditionIndicator.isExtremelyLow())
                return false;

            if(!__instance.getComponentType().isProducer())
                return false;

            if(!__instance.isOperational())
                return false;

            if(!__instance.canProduce())
                return false;

            if(__instance.getResourceContainer().getResourceCount() == 0)
                return false;

            return true;
        }

        private static void consumeResources(List<ResourceType> resourceConsumption, ResourceContainer mResourceContainer)
        {
            int highestCountResourceIndex = Resource.getHighestCountResourceIndex(resourceConsumption);
            for(int i = 0; i < resourceConsumption.Count; i++) {
                ResourceType resourceType = resourceConsumption[(i + highestCountResourceIndex) % resourceConsumption.Count];
                if(mResourceContainer.contains(resourceType)) 
                    mResourceContainer.remove(resourceType)?.destroy();
            }
        }
    }
}
