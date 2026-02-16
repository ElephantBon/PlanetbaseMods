using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static Planetbase.LandingShip;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Module), "updateMining")]
    public class updateMiningPatch
    {
        public static bool Prefix(Module __instance, float timeStep)
        {
            // Replace producing of tunnel construction site

            if(!(__instance.getModuleType() is ModuleTypeTunnelConstruction))
                return true;    // Vanilla mechanism

            if(!__instance.isBuilt())
                return false;

            // Set indicator icon before first worker enters
            Indicator mProductionProgressIndicator;

            // Replace icon of progress
            mProductionProgressIndicator = CoreUtils.GetMember<Module, Indicator>("mProductionProgressIndicator", __instance);
            if(mProductionProgressIndicator == null)
                return false;
            mProductionProgressIndicator.setIcon(ContentManager.IconTunnel);

            var mInteractions = CoreUtils.GetMember<Module, List<Interaction>>("mInteractions", __instance);
            if(mInteractions == null || mInteractions.Count == 0)
                return false;

            if(!mProductionProgressIndicator.isValidValue())
                mProductionProgressIndicator.setValue(0f);

            Character character = mInteractions[0].getCharacter();
            float num = 0.5f + (float)__instance.getInteractionCount() * 0.5f;
            if(mProductionProgressIndicator.increase(timeStep * character.getWorkSpeed() * num / 150f / HeyHowAboutTheBasement.processTimeFactorTunnelConstruction)) {
                // Remove tunnel construction site
                HeyHowAboutTheBasement.replaceTunnelConstructions.Add(__instance);
            }

            return false;   // Never do vanilla mechanism of mine
        }
    }
}
