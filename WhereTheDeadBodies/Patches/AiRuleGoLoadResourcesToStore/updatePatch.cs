using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

using WhereTheDeadBodies.Objects;
using static Planetbase.Resource;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Prevent people go to pick up resources when there is no correct type of storage available
    /// </summary>
    [HarmonyPatch(typeof(AiRuleGoLoadResourcesToStore), nameof(AiRuleGoLoadResourcesToStore.update))]
    public class updatePatch
    {
        public static bool Prefix(AiRuleGoLoadResourcesToStore __instance, ref bool __result, Character character)
        {
            __result = false;
            if(__instance.hasCarryPriority(character) && __instance.canWork(character)) {
                if(goGetResourceToStore(__instance, character, ResourceTypeList.MedicalSuppliesInstance)) {
                    __result = true;
                }
                else
                if(goGetResourceToStore(__instance, character, ResourceTypeList.GunInstance)) {
                    __result = true;                    
                }
                else { 
                    if(StoreResourceTask.tasks.Count == 0)
                        StoreResourceTask.update();

                    Resource resource = null;
                    Module module = null;
                    var task = StoreResourceTask.findBest(character, out resource, out module);
                    if(resource != null && module != null) {
                        StoreResourceTask.clear();
                        __result = goTarget(character, resource, module);
                    }
                }
            }

            return false;
        }


        public class StoreResourceTask
        {
            public Resource resource;
            public List<Module> canidateStorages;

            public static List<StoreResourceTask> tasks = new List<StoreResourceTask>();

            public static StoreResourceTask findBest(Character character, out Resource resultResource, out Module resultModule)
            {
                StoreResourceTask resultTask = null;
                resultResource = null;
                resultModule = null;

                // Find best resource
                float num = float.MaxValue;
                foreach(var task in tasks) {
                    var resource = task.resource;
                    if(resource.getPotentialUserCount(character) > 0)
                        continue;

                    float num2 = (resource.getPosition() - character.getPosition()).magnitude;
                    if(character.getLocation() == resource.getLocation()) {
                        num2 -= 20f;
                    }

                    num2 += CoreUtils.GetMember<Resource, Indicator>("mConditionIndicator", resource).getValue() * 100f;
                    if(num2 < num && CoreUtils.InvokeMethod<Resource, bool>("calculateReachable", resource)) {
                        num = num2;
                        resultTask = task;
                        resultResource = resource;
                    }
                }

                // Find best module for the resource
                if(resultTask != null) {
                    WhereTheDeadBodies.ModEntry.Logger.Log($"findBest, character={character.getName()}, resource={resultResource.getName()}, storageCount={resultTask.canidateStorages.Count}");
                    num = float.MaxValue;
                    var position = character.getPosition();
                    foreach(var module in resultTask.canidateStorages) {
                        if(module.isOperational() && module.isSurvivable(character) && module.getEmptyStorageSlotCount() > module.getPotentialUserCount(character)) {
                            float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
                            if(sqrMagnitude < num) {
                                resultModule = module;
                                num = sqrMagnitude;
                                WhereTheDeadBodies.ModEntry.Logger.Log($"findBest, character={character.getName()}, resource={resultResource.getName()}, module={resultModule.getName()}");
                            }
                        }
                    }
                }

                return resultTask;
            }

            internal static void add(Resource resource, List<Module> storages)
            {
                tasks.Add(new StoreResourceTask() { resource = resource, canidateStorages = storages});
            }

            internal static void clear()
            {
                tasks.Clear();
            }

            internal static void remove(StoreResourceTask task)
            {
                tasks.Remove(task);
            }

            internal static void update()
            {
                clear();

                // Find all storages have empty slots
                var storages = Module.getCategoryModules(Module.Category.Storage)?.Where(x => x.getEmptyStorageSlotCount() > 0).ToList();
                if(storages == null || storages.Count == 0)
                    return;

                // Find all idle resources
                var resources = new List<Resource>();
                CoreUtils.GetMember<Resource, List<Resource>>("mResources").ForEach(resource => {
                    bool flag = !resource.isEmbedded();
                    if(resource.getLocation() == Location.Exterior && !Singleton<SecurityManager>.getInstance().isGoingOutsideAllowed()) {
                        flag = false;
                    }
                    else if(!flag && resource.getContainer().getParent() is ConstructionComponent constructionComponent && constructionComponent.hasFlag(1048576) && constructionComponent.canProduceResource(resource.getResourceType())) {
                        flag = true;
                    }

                    if(flag && !resource.isTraded() && resource.getState() == State.Idle) {
                        resources.Add(resource);
                    }
                });

                // Create tasks
                var normalStorages = storages.Where(x => !(x.getModuleType() is ModuleTypeMorgue)).ToList();
                var morgues = storages.Where(x => x.getModuleType() is ModuleTypeMorgue).ToList();
                foreach(var resource in resources) {
                    if(resource.getResourceType() is Corpse) {
                        if(morgues.Count > 0) 
                            add(resource, morgues);
                    }
                    else {
                        if(normalStorages.Count > 0)
                            add(resource, normalStorages);
                    }
                }
            }
        }

        private static bool goGetResourceToStore(AiRuleGoLoadResourcesToStore instance, Character character, ResourceType resourceType)
        {
            return CoreUtils.InvokeMethod<AiRuleGoLoadResourcesToStore, bool>("goGetResourceToStore", instance, character, resourceType);
        }

        private static bool goTarget(Character character, Selectable targetSelectable, Selectable finalTarget = null, Location overrideLocation = Location.Unknown)
        {
            WhereTheDeadBodies.ModEntry.Logger.Log($"goTarget, character={character.getName()}, target={targetSelectable?.ToString()}, finalTarget={finalTarget?.ToString()}");
            System.Reflection.MethodInfo method = typeof(AiRule).GetMethod("goTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, 
                    new Type[] { typeof(Character), typeof(Selectable), typeof(Selectable), typeof(Location) }, null);
            return (bool)method.Invoke(null, new object[] { character, targetSelectable, finalTarget, overrideLocation });
        }
    }
}
