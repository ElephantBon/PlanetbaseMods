using GeniusEngineers.Objects;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), "updateProduction")]
    internal class updateProductionPatch
    {
        public static bool Prefix(ConstructionComponent __instance, float timeStep)
        {
            // Operator doesn't necessary for nuclear generator 

            var mComponentType = __instance.getComponentType();
            if(!(mComponentType is ResearchTable))
                return true; // original

            var mParentConstruction = __instance.getParentConstruction();
            var mConditionIndicator = CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", __instance);
            if(mParentConstruction.isEnabled() && !mConditionIndicator.isExtremelyLow() && mComponentType.isProducer() && __instance.isOperational() && __instance.canProduce() && (!mComponentType.hasFlag(8) || __instance.anyInteractions())) {
                float num = 1f;
                if(__instance.anyInteractions()) {
                    num *= __instance.getFirstInteraction().getCharacter().getWorkSpeed();
                }

                if(mConditionIndicator.isLow()) {
                    num *= 0.5f;
                }

                float resourceProductionPeriod = mComponentType.getResourceProductionPeriod();
                var mProductionProgress = CoreUtils.GetMember<ConstructionComponent, Indicator>("mProductionProgress", __instance);
                if( mProductionProgress.increase(num * timeStep / resourceProductionPeriod)) {
                    // Complete production
                    
                    // Consume resource
                    var resourceConsumption = mComponentType.getResourceConsumption();
                    if(resourceConsumption != null) {
                        int highestCountResourceIndex = Resource.getHighestCountResourceIndex(resourceConsumption);
                        for(int i = 0; i < resourceConsumption.Count; i++) {
                            ResourceType resourceType = resourceConsumption[(i + highestCountResourceIndex) % resourceConsumption.Count];
                            var mResourceContainer = __instance.getResourceContainer();
                            if(mResourceContainer.contains(resourceType)) {
                                mResourceContainer.remove(resourceType)?.destroy();
                            }
                        }
                    }

                    // Update research level
                    Main.CompleteResearch((Main.Research)__instance.getProducedItemIndex());

                    // Reset indicator
                    mProductionProgress.setValue(0f);
                }
            }

            return false;
        }
    }
}
