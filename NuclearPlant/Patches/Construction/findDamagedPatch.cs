using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using NuclearPlant.Objects;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(Construction), nameof(Construction.findDamaged))]
    internal class findDamagedPatch
    {
        public static bool Prefix(ref Construction __result, Character character, Specialization specialization)
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

                if(construction.isDamaged(specialization) && construction.isBuilt() && construction.anyBuiltLinks() && construction.getPotentialUserCount(character) == 0) {
                    float num2 = (construction.getPosition() - character.getPosition()).magnitude;
                    if(construction.isHighPriority()) {
                        num2 -= 100f;
                    }

                    var mConditionIndicator = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", construction);
                    if(mConditionIndicator.isVeryLow()) {
                        num2 -= 100f;
                    }
                    else if(mConditionIndicator.isExtremelyLow()) {
                        num2 -= 200f;
                    }

                    if(num2 < num) {
                        num = num2;
                        result = construction;
                    }
                }
            }

            __result = result;
            return false;
        }
    }
}
