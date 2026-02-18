using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;

namespace WhereTheDeadBodies.Objects
{
    public class Incinerator : ComponentType
    {
        public const string Name = "Incinerator";

        public Incinerator()
        {
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            mIcon = ContentManager.IconIncinerator;
            mResourceConsumption = new List<ResourceType>();
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Corpse>());
            addResourceProduction<Remains>();
            mEmbeddedResourceCount = 1;
            mResourceProductionPeriod = 120f;
            mPowerGeneration = -10000;
            mFlags = 1572904;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            addUsageAnimation(CharacterAnimationType.WorkStanding);

            mName = Name;
            mPrefabName = "PrefabMetalProcessor";

            initStrings();
        }
        public static void RegisterString()
        {
            StringUtils.RegisterString("component_incinerator", Name);
        }
    }
}
