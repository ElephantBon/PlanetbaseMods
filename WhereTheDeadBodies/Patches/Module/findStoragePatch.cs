using HarmonyLib;
using Planetbase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    /// <summary>
    /// Restrict corpse to morgue, other resources to storage
    /// </summary>
    [HarmonyPatch(typeof(Module), nameof(Module.findStorage))]
    public class findStoragePatch
    {
        public static bool Prefix(ref Module __result, Character character)
        {
            float num = float.MaxValue;            
            Vector3 position = character.getPosition();
            List<Module> list = Module.getCategoryModules(Module.Category.Storage);
            if(list != null) {
                // Choose normal Storage or Morgue depends on current loadded resource type
                var loadedResource = character.getLoadedResource();
                if(loadedResource != null) {
                    if(loadedResource.getResourceType() is Corpse) {
                        list = list.Where(x => x.getModuleType() is ModuleTypeMorgue).ToList();
                    }
                    else {
                        list = list.Where(x => !(x.getModuleType() is ModuleTypeMorgue)).ToList();
                    }
                }

                int count = list.Count;
                for(int i = 0; i < count; i++) {
                    Module module = list[i];
                    if(module.isOperational() && module.isSurvivable(character) && module.getEmptyStorageSlotCount() > module.getPotentialUserCount(character)) {
                        float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
                        if(sqrMagnitude < num) {
                            __result = module;
                            num = sqrMagnitude;
                        }
                    }
                }
            }

            return false;
        }
    }
}
