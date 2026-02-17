using Planetbase;
using UnityEngine;
using HarmonyLib;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Bot), nameof(Bot.getMaxSpeed))]
    public class BotPatch
    {
        public static bool Prefix(ref float __result, Bot __instance)
        {
            float num = 6f * __instance.getSpecialization().getSpeedFactor();
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
