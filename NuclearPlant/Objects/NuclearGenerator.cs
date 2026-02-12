using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuclearPlant.Objects
{
    public class NuclearGenerator : ComponentType
    {
        public const string Name = "Generator";

        public NuclearGenerator()
        {
            mConstructionCosts = new ResourceAmounts();

#if DEBUG
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
#else
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 2);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 2);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 4);
#endif

            mResourceConsumption = new List<ResourceType>();
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<UraniumRod>());
            addResourceProduction<ResourcePower>();
            mResourceProductionPeriod = 14400f;
            mEmbeddedResourceCount = 12;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Engineer>();
            mFlags |= 1048872;
            addUsageAnimation(CharacterAnimationType.WorkStanding);

            mIcon = ContentManager.IconPower;
            mPrefabName = "PrefabMetalProcessor";
            mName = Name;

            initStrings();
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("component_nuclear_generator", Name);
        }
    }
}
