using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(ModuleType))]
    public class ModuleTypePatch
    {
        [HarmonyPatch(nameof(ModuleType.getPowerStorageCapacity))]
        [HarmonyPrefix]
        public static bool getPowerStorageCapacity(ref int __result, ModuleType __instance, float sizeFactor)
        {
            var mPowerStorageCapacity = CoreUtils.GetMember<ModuleType, int>("mPowerStorageCapacity", __instance);
            __result = Mathf.RoundToInt((float)mPowerStorageCapacity * sizeFactor * (100 + Main.ExtraPowerStorage) / 100);
            return false;
        }

        [HarmonyPatch(nameof(ModuleType.getWaterStorageCapacity))]
        [HarmonyPrefix]
        public static bool getWaterStorageCapacity(ref int __result, ModuleType __instance, float sizeFactor)
        {
            var mWaterStorageCapacity = CoreUtils.GetMember<ModuleType, int>("mWaterStorageCapacity", __instance);
            __result = Mathf.RoundToInt((float)mWaterStorageCapacity * sizeFactor * (100 + Main.ExtraWaterStorage) / 100);
            return false;
        }
    }
}
