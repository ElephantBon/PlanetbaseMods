using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), "updateAnimation")]
    internal class updateAnimationPatch
    {
        public static bool Prefix(ConstructionComponent __instance)
        {
            if(!(__instance.getComponentType() is NuclearGeneratorPowered))
                return true;

            var mAnimation = CoreUtils.GetMember<ConstructionComponent, Animation>("mAnimation", __instance);
            if(mAnimation != null) {
                if(__instance.isEnabled()) {
                    if(!mAnimation.isPlaying)
                        mAnimation.Play();
                }
                else {
                    if(mAnimation.isPlaying)
                        mAnimation.Stop();
                }
            }

            return false;
        }
    }
}
