using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), "updateDescriptionItems")]
    internal class updateDescriptionItemsPatch
    {
        public static bool Prefix(ConstructionComponent __instance)
        {
            var isEnable = __instance.isEnabled() && __instance.getParentConstruction().isEnabled();

            // Power
            {
                var powerGeneration = (isEnable ? __instance.getComponentType().getPowerGeneration() : 0);
                var text = Util.gridResourceToString(Math.Abs(powerGeneration));
                var icon = (powerGeneration <= 0 ? ResourceList.StaticIcons.PowerConsumption : ResourceList.StaticIcons.PowerGeneration);
                var tooltip = "Power" + (powerGeneration <= 0 ? " Consumption: " : " Generation: ") + text;

                var mPowerGenerationItem = CoreUtils.GetMember<ConstructionComponent, DescriptionItem>("mPowerGenerationItem", __instance);
                if(mPowerGenerationItem == null) {
                    if(powerGeneration != 0) {
                        mPowerGenerationItem = new DescriptionItem(text, icon, tooltip);
                        var mDescriptionItems = CoreUtils.GetMember<ConstructionComponent, ListWrapper<DescriptionItem>>("mDescriptionItems", __instance);
                        mDescriptionItems.add(mPowerGenerationItem);
                        CoreUtils.SetMember("mPowerGenerationItem", __instance, mPowerGenerationItem);
                        CoreUtils.SetMember("mDescriptionItems", __instance, mDescriptionItems);
                    }
                }
                else {
                    mPowerGenerationItem.setText(text);
                    mPowerGenerationItem.setIcon(icon);
                    mPowerGenerationItem.setTooltip(tooltip);
                }
            }


            // Water
            {
                var waterGeneration = (isEnable ? __instance.getComponentType().getWaterGeneration() : 0);
                var text = Util.gridResourceToString(Math.Abs(waterGeneration));
                var icon = (waterGeneration <= 0 ? ResourceList.StaticIcons.WaterConsumption : ResourceList.StaticIcons.WaterGeneration);
                var tooltip = "Water" + (waterGeneration <= 0 ? " Consumption: " : " Generation: ") + text;

                var mWaterGenerationItem = CoreUtils.GetMember<ConstructionComponent, DescriptionItem>("mWaterGenerationItem", __instance);
                if(mWaterGenerationItem == null) {
                    if(waterGeneration != 0) {
                        mWaterGenerationItem = new DescriptionItem(text, icon, tooltip);
                        var mDescriptionItems = CoreUtils.GetMember<ConstructionComponent, ListWrapper<DescriptionItem>>("mDescriptionItems", __instance);
                        mDescriptionItems.add(mWaterGenerationItem);
                        CoreUtils.SetMember("mWaterGenerationItem", __instance, mWaterGenerationItem);
                        CoreUtils.SetMember("mDescriptionItems", __instance, mDescriptionItems);
                    }
                }
                else {
                    mWaterGenerationItem.setText(text);
                    mWaterGenerationItem.setIcon(icon);
                    mWaterGenerationItem.setTooltip(tooltip);
                }
            }

            return false;
        }
    }
}
