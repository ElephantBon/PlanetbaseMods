using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Construction), nameof(Construction.setEnabled))]
    public class ConstructionPatch
    {
        public static void Postfix(Construction __instance, bool enabled)
        {
            // Connection of moving walkway sync with controllers
            if(__instance is Module module && module.getModuleType() is ModuleTypeMovingWalkwayController) {
                var mLinks = CoreUtils.GetMember<Module, List<Construction>>("mLinks", module);
                foreach(var link in mLinks) {
                    if(link is Connection connection) {
                        if(connection.getModule1().getModuleType() is ModuleTypeMovingWalkwayController
                        && connection.getModule2().getModuleType() is ModuleTypeMovingWalkwayController) {
                            connection.setEnabled(connection.getModule1().isEnabled() && connection.getModule2().isEnabled());
                        }
                    }
                }
            }
        }
    }
}
