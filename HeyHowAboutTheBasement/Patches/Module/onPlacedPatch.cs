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
    [HarmonyPatch(typeof(Module), nameof(Module.onPlaced))]
    public class onPlacedPatch
    {
        public static void Postfix(Module __instance)
        {
            if(__instance.getModuleType() is ModuleTypeTunnelConstruction) {
                // If the placed module is tunnel under construction, try to find the nearest tunnel entrance to build connection immediately
                var entrance = CoreUtils.GetMember<Module, List<Module>>("mModules")
                    .Where(x => x.getModuleType() is ModuleTypeTunnelEntrance && Connection.canLink(__instance, x))
                    .OrderBy(x => x.distanceTo(__instance.getPosition()))
                    .FirstOrDefault();

                if(entrance != null) {
                    var entrance2 = Main.ReplaceTunnelConstructionWithEntrance(__instance);
                    Connection.create(entrance2, entrance).onBuilt();
                    Main.deletingModules.Add(__instance); // destroy later to avoid exception
                }
            }
            else
            if(__instance.getModuleType() is ModuleTypeAirlock) {
                // If the placed module is airlock and it connects to tunnel entrance which only has tunnel connection, then immediately build the airlock and connection
                var nearestModule = CoreUtils.GetMember<Module, List<Module>>("mModules")
                    .Where(x => Connection.canLink(__instance, x))
                    .OrderBy(x => x.distanceTo(__instance.getPosition()))
                    .FirstOrDefault();

                if(nearestModule != null && nearestModule.getModuleType() is ModuleTypeTunnelEntrance) {
                    var entrance = nearestModule;

                    //Debug.Log($"HeyHowAboutTheBasement: airlock linking entrance. linkCount={entrance.getLinkCount()}, otherModuleType={((Module)entrance.getLink(0)).getModuleType().ToString()}");
                    if(entrance.getLinkCount() == 1 && entrance.getLink(0) is Connection connection) {
                        Module anotherModule;
                        if(connection.getModule1() == entrance)
                            anotherModule = connection.getModule2();
                        else
                            anotherModule = connection.getModule1();

                        if(anotherModule.getModuleType() is ModuleTypeTunnelEntrance) {
                            __instance.onBuilt();
                            Connection.create(__instance, entrance).onBuilt();
                        }
                    }
                }
            }
        }
    }
}
