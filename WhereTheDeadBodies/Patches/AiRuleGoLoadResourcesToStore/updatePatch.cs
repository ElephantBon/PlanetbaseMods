using HarmonyLib;
using Planetbase;
using UnityEngine;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    //[HarmonyPatch(typeof(AiRuleGoLoadResourcesToStore), nameof(AiRuleGoLoadResourcesToStore.update))]
    //public class updatePatch
    //{
    //    public static bool Prefix(AiRuleGoLoadResourcesToStore __instance, ref bool __result, Character character)
    //    {
    //        if(__instance.hasCarryPriority(character) && __instance.canWork(character)) {
    //            if(goGetResourceToStore(character, ResourceTypeList.MedicalSuppliesInstance)) {
    //                __result = true;
    //                return false;
    //            }
    //            if(goGetResourceToStore(character, ResourceTypeList.GunInstance)) {
    //                __result = true;
    //                return false;
    //            }
    //            Module module = Module.findStorage(character);
    //            if(module != null) {
    //                var targetResourceType = (module.getModuleType() is ModuleTypeMorgue ? TypeList<ResourceType, ResourceTypeList>.find<Corpse>() : null);
    //                Resource resource = Resource.findStorable(character, targetResourceType, includeStorages: false);

    //                if(resource != null) {
    //                    __result = goTarget(character, resource, module);
    //                    return false;
    //                }
    //            }
    //        }
    //        return false;
    //    }
    //    private static bool goGetResourceToStore(Character character, ResourceType resourceType)
    //    {
    //        ConstructionComponent constructionComponent = Module.findStorageComponent(character, resourceType);
    //        if(constructionComponent != null) {
    //            Resource resource = Resource.findStorable(character, resourceType, includeStorages: true);
    //            if(resource != null) {
    //                return goTarget(character, resource, constructionComponent);
    //            }
    //        }

    //        return false;
    //    }
    //    private static bool goTarget(Character character, Selectable targetSelectable, Selectable finalTarget = null, Location overrideLocation = Location.Unknown)
    //    {
    //        Target target = new Target(targetSelectable);
    //        target.setRadius(targetSelectable.getRadius() + character.getRadius());
    //        return goTarget(character, target, finalTarget, overrideLocation);
    //    }
    //    private static bool goTarget(Character character, Target target, Selectable finalTarget = null, Location overrideLocation = Location.Unknown)
    //    {
    //        Selectable selectable = target.getSelectable();
    //        Location location = ((overrideLocation == Location.Unknown) ? target.getLocation() : overrideLocation);
    //        if(character.getLocation() == location) {
    //            if(location != Location.Exterior) {
    //                character.startWalking(target, finalTarget);
    //                return true;
    //            }

    //            int exteriorColorIndex = NavigationGraph.getExteriorColorIndex(target.getPosition());
    //            int exteriorColorIndex2 = NavigationGraph.getExteriorColorIndex(character.getPosition());
    //            if(exteriorColorIndex == exteriorColorIndex2) {
    //                character.startWalking(target, finalTarget);
    //                return true;
    //            }

    //            if(exteriorColorIndex == -1) {
    //                Debug.LogWarning("Trying to reach isolated target: " + character.getName() + " - " + target);
    //                return false;
    //            }
    //        }

    //        if(character.isWaitingForAirlock()) {
    //            return true;
    //        }

    //        return goToBestAirlock(character, selectable, finalTarget);
    //    }
    //    private static bool goToBestAirlock(Character character, Selectable targetSelectable, Selectable finalTarget = null)
    //    {
    //        Module module = null;
    //        module = ((character.getLocation() != 0) ? Module.findClosestAirlock(character, character.getPosition()) : Module.findClosestAirlock(character, targetSelectable.getPosition()));
    //        if(module != null) {
    //            if(!character.isWaitingForAirlock(module)) {
    //                if(targetSelectable == finalTarget) {
    //                    Debug.LogError("goToBestAirlock - Target is the same as final target");
    //                }

    //                if(finalTarget != null) {
    //                    character.startWalking(getAirlockTarget(module, character), new Selectable[2] { targetSelectable, finalTarget });
    //                }
    //                else {
    //                    character.startWalking(getAirlockTarget(module, character), targetSelectable);
    //                }
    //            }

    //            return true;
    //        }

    //        return false;
    //    }
    //    private static Target getAirlockTarget(Module airlock, Character character)
    //    {
    //        Vector3 position = airlock.getPoint("exit_point").position;
    //        if(character.getLocation() == Location.Interior) {
    //            position = airlock.getPoint("entry_point").position;
    //        }

    //        Target target = new Target(airlock, position);
    //        target.setRadius(1f);
    //        return target;
    //    }

    //}
}
