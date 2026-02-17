using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace WhereTheDeadBodies
{
    [HarmonyPatch(typeof(Character), "updateDead")]
    public class updateDeadPatch
    {
        public static bool Prefix(Character __instance, float timeStep)
        {
            if(__instance.isLoaded()) {
                __instance.unloadResource(Resource.State.Idle);
            }

            var mDeadTime = CoreUtils.GetMember<Character, float>("mDeadTime", __instance);
            mDeadTime += timeStep;
            if(mDeadTime > 10f) {
                __instance.recycle();
                CoreUtils.InvokeMethod("destroyDeferred", __instance);
            }
            else {
                CoreUtils.SetMember<Character, float>("mDeadTime", __instance, mDeadTime);
            }

            return false; // Stop vanilla mechanism
        }
    }
}
