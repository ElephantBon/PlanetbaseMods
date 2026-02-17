using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WhereTheDeadBodies.Objects;

namespace WhereTheDeadBodies.Patches
{
    //[HarmonyPatch(typeof(Module), nameof(Module.findStorage))]
    //public class findStoragePatch
    //{
    //    public static bool Prefix(ref Module __result, Character character)
    //    {
    //        var mModuleCategories = CoreUtils.GetMember<Module, List<Module>[]>("mModuleCategories");

    //        float num = float.MaxValue;
    //        Module result = null;
    //        Vector3 position = character.getPosition();
    //        List<Module> list = mModuleCategories[2];
    //        if(list != null) {
    //            int count = list.Count;
    //            for(int i = 0; i < count; i++) {
    //                Module module = list[i];

    //                var loadedResource = character.getLoadedResource();
    //                if(loadedResource != null) {
    //                    bool isMorgue = module.getModuleType() is ModuleTypeMorgue;
    //                    bool isCorpse = loadedResource.getResourceType() is Corpse;

    //                    if(isMorgue != isCorpse)
    //                        continue;
    //                }

    //                if(module.isOperational() && module.isSurvivable(character) && module.getEmptyStorageSlotCount() > module.getPotentialUserCount(character)) {
    //                    float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
    //                    if(sqrMagnitude < num) {
    //                        result = module;
    //                        num = sqrMagnitude;
    //                    }
    //                }
    //            }
    //        }

    //        __result = result;
    //        return false;
    //    }
    //}
}
