using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(Module), "updateMining")]
    public class updateMiningPatch
    {
        public static bool Prefix(Module __instance, float timeStep)
        {
            if(!__instance.hasFlag(2) || !__instance.isBuilt()) 
                return false;

            var mInteractions = CoreUtils.GetMember<Module, List<Interaction>>("mInteractions", __instance);
            if(mInteractions == null || mInteractions.Count == 0 || mInteractions.Where(x=>x is InteractionWork).Count() == 0)
                return false;

            var mProductionProgressIndicator = CoreUtils.GetMember<Module, Indicator>("mProductionProgressIndicator", __instance);
            mProductionProgressIndicator.setIcon(ResourceTypeList.OreInstance.getIcon());
            if(!mProductionProgressIndicator.isValidValue()) 
                mProductionProgressIndicator.setValue(0f);

            Character character = mInteractions[0].getCharacter();
            float num = 0.5f + (float)mInteractions.Count * 0.5f;
            if(mProductionProgressIndicator.increase(timeStep * character.getWorkSpeed() * num / 150f)) {
                mProductionProgressIndicator.setValue(0f);
                if(findValidProductionPosition(__instance, out var dropPosition)) {
                    bool canMineUraniumOre;
                    if(Main.settings.uraniumOreRequiresTech) {
                        // Require tech to find uranium ore
                        var tech = TypeList<Tech, TechList>.find<TechNuclearPlant>();
                        canMineUraniumOre = Singleton<TechManager>.getInstance().isAcquired(tech);
                    }
                    else {
                        canMineUraniumOre = true;
                    }
                    var resourceIType = (canMineUraniumOre && Main.random.NextDouble() <= Main.ProbabilityUraniumOre ?
                        ResourceTypeList.find<UraniumOre>() : ResourceTypeList.OreInstance);
                    Resource resource = Resource.create(resourceIType, dropPosition, Location.Exterior);
                    resource.setRotation(__instance.getTransform().rotation);
                    resource.drop(Resource.State.Idle);
                }
            }

            return false;
        }

        private static bool findValidProductionPosition(Module module, out Vector3 dropPosition)
        {
            var position = module.getPosition();
            var transform = module.getTransform();
            var radius = module.getRadius();

            float num = Singleton<TerrainGenerator>.getInstance().getFloorHeight() + 2.5f;
            dropPosition = position + transform.forward * radius * 1.5f + transform.right * 2f;
            if(PhysicsUtil.findFloor(dropPosition, out var terrainPosition, 1280) && terrainPosition.y <= num) {
                return true;
            }

            dropPosition = position + transform.forward * radius * 1.5f - transform.right * 2f;
            if(PhysicsUtil.findFloor(dropPosition, out terrainPosition, 1280) && terrainPosition.y <= num) {
                return true;
            }

            dropPosition = Vector3.zero;
            return false;
        }
    }
}
