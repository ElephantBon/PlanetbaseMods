using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Human), nameof(Human.getMaxSpeed))]
    public class HumanPatch
    {
        public static bool Prefix(ref float __result, Human __instance)
        {
            var mIndicators = CoreUtils.GetMember<Character, Indicator[]>("mIndicators", __instance);
            float num = 4f * (0.5f + mIndicators[(int)(CharacterIndicator.Nutrition)].getValue() * 0.3f + mIndicators[(int)(CharacterIndicator.Sleep)].getValue() * 0.3f);
            if(__instance.getLoadedResource() != null) {
                num *= 0.75f;
            }
            if(__instance.getLocation() == Location.Exterior) {
                num *= 0.75f;
            }
            if(__instance.getCondition() != null) {
                num *= 0.75f;
            }
            if(__instance.getLocation() == Location.Exterior) {
                Disaster stormInProgress = Singleton<DisasterManager>.getInstance().getStormInProgress();
                if(stormInProgress != null) {
                    float num2 = Mathf.Lerp(1f, 0.25f, stormInProgress.getIntensity());
                    num *= num2;
                }
            }

            num = HeyHowAboutTheBasement.IncreaseSpeedOnWalkway(__instance, num);

            __result = num;
            return false;
        }
    }
}
