using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(Construction), nameof(Construction.findExtremelyDamagedInterior))]
    internal class findExtremelyDamagedInteriorPatch
    {
        public static bool Prefix(ref Construction __result, Character character)
        {
            float num = float.MaxValue;
            Construction result = null;
            var mConstructions = CoreUtils.GetMember<Construction, List<Construction>>("mConstructions");
            for(int i = 0; i < mConstructions.Count; i++) {
                Construction construction = mConstructions[i];

                // Nuclear plant cannot be repaired by engineers
                var module = construction as Module;
                if(module != null && module.getModuleType() is ModuleTypeNuclearPlant)
                    continue;

                if(construction.isExtremelyDamaged() && construction.isBuilt() && construction.getLocation() == Location.Interior && construction.anyBuiltLinks() && construction.getPotentialUserCount(character) == 0) {
                    float magnitude = (construction.getPosition() - character.getPosition()).magnitude;
                    if(magnitude < num) {
                        num = magnitude;
                        result = construction;
                    }
                }
            }

            __result = result;
            return false;
        }

        //public static Construction findDamaged(Character character, Specialization specialization)
        //{
        //    float num = float.MaxValue;
        //    Construction result = null;
        //    for(int i = 0; i < mConstructions.Count; i++) {
        //        Construction construction = mConstructions[i];
        //        if(construction.isDamaged(specialization) && construction.isBuilt() && construction.anyBuiltLinks() && construction.getPotentialUserCount(character) == 0) {
        //            float num2 = (construction.getPosition() - character.getPosition()).magnitude;
        //            if(construction.isHighPriority()) {
        //                num2 -= 100f;
        //            }
        //            if(construction.mConditionIndicator.isVeryLow()) {
        //                num2 -= 100f;
        //            }
        //            else if(construction.mConditionIndicator.isExtremelyLow()) {
        //                num2 -= 200f;
        //            }
        //            if(num2 < num) {
        //                num = num2;
        //                result = construction;
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}
