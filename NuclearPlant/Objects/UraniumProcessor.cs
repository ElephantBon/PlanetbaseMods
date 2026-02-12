using NuclearPlant;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuclearPlant.Objects
{
    public class UraniumProcessor : ComponentType
    {
        public const string Name = "Uranium Processor";

        public UraniumProcessor()
        {
            mConstructionCosts = new ResourceAmounts();
            mResourceConsumption = new List<ResourceType>();

#if DEBUG
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<UraniumOre>());
            mResourceProductionPeriod = 20f;
#else
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 2);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Spares>());
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Spares>());
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Spares>());
            mResourceProductionPeriod = 360f;
#endif


            mRequiredTech = TypeList<Tech, TechList>.find<TechNuclearPlant>();

            addResourceProduction<UraniumRod>();
            addResourceProduction<UraniumRod>();
            addResourceProduction<UraniumRod>();

            mEmbeddedResourceCount = 4;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Engineer>();
            mFlags |= 1048872;
            mPowerGeneration = -1500;
            addUsageAnimation(CharacterAnimationType.WorkStanding);

            mIcon = ContentManager.IconUraniumProcessor;
            mPrefabName = "PrefabMetalProcessor";
            mName = Name;

            initStrings();
        }
        public static void RegisterString()
        {
            StringUtils.RegisterString("component_uranium_processor", Name);
        }
    }
}
