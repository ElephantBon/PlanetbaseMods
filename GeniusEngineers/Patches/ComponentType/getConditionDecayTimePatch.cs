using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(ComponentType), nameof(ComponentType.getConditionDecayTime))]
    public class getConditionDecayTimePatch
    {
        public static bool Prefix(ref float __result, ComponentType __instance)
        {
            float num = CoreUtils.GetMember<ComponentType, float>("mConditionDecayTime", __instance); ;
            if(__instance.hasFlag(ComponentType.FlagFastProduction)) {
                num *= 0.75f;
            }

            if(__instance.hasFlag(ComponentType.FlagSlowProduction)) {
                num *= 1.25f;
            }

            __result = num * (100 + Main.ExtraVegetableLife) / 100;
            return false;
        }
    }
}
