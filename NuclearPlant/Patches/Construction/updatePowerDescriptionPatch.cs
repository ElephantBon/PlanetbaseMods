using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NuclearPlant.Patches
{
    /// <summary>
    /// Display power generation and water consumption based on current functional Nuclear Reactors
    /// </summary>
    [HarmonyPatch(typeof(Construction), nameof(Construction.updatePowerDescription))]
    internal class updatePowerDescriptionPatch
    {
        public static bool Prefix(Construction __instance)
        {
            // Only apply to Nuclear Plants
            if(!(__instance is Module module && module.getModuleType() is ModuleTypeNuclearPlant))
                return true;

            var mPowerDescriptionItem = CoreUtils.GetMember<Construction, DescriptionItem>("mPowerDescriptionItem", __instance);
            if(mPowerDescriptionItem != null) {
                int powerGeneration = __instance.getPowerGeneration();
                int maxPowerGeneration = __instance.getMaxPowerGeneration();
                int powerCollection = __instance.getPowerCollection();
                Texture2D icon;
                string text;
                string tooltip;
                if(powerCollection > 0) {
                    icon = ResourceList.StaticIcons.PowerGeneration;
                    text = Util.gridResourceToString(powerCollection);
                    tooltip = StringList.get("tooltip_power_collection", Util.gridResourceToString(powerCollection));
                }
                else if(maxPowerGeneration >= 0) {
                    int maxPowerGeneration2 = __instance.getMaxPowerGeneration();
                    icon = ResourceList.StaticIcons.PowerGeneration;
                    text = Util.gridResourceToString(powerGeneration) + " / " + Util.gridResourceToString(maxPowerGeneration2);
                    tooltip = StringList.get("tooltip_power_generation", Util.powerToString(powerGeneration), Util.powerToString(maxPowerGeneration2));
                }
                else {
                    int componentPowerGeneration = getComponentPowerGeneration(__instance);
                    icon = componentPowerGeneration <= 0 ?
                        ResourceList.StaticIcons.PowerConsumption :
                        ResourceList.StaticIcons.PowerGeneration;
                    text = Util.gridResourceToString(Math.Abs(powerGeneration));

                    // Tooltip not modified
                    tooltip = ((componentPowerGeneration >= 0) ? StringList.get("tooltip_power_consumption", Util.powerToString(-powerGeneration)) : StringList.get("tooltip_power_consumption_components", Util.powerToString(-powerGeneration), Util.powerToString(-componentPowerGeneration)));
                }
                mPowerDescriptionItem.setText(text);
                mPowerDescriptionItem.setTooltip(tooltip);
                mPowerDescriptionItem.setIcon(icon);
            }

            return false;
        }

        private static int getComponentPowerGeneration(Construction construction)
        {
            int num = 0;
            var mComponents = construction.getComponents();
            int count = mComponents.Count;
            for(int i = 0; i < count; i++) {
                ConstructionComponent constructionComponent = mComponents[i];
                if(constructionComponent.isBuilt() && constructionComponent.isEnabled()) {
                    num += constructionComponent.getComponentType().getPowerGeneration();
                }
            }

            return num;
        }
    }
}
