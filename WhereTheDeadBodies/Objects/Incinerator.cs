using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WhereTheDeadBodies.Objects
{
    public class Incinerator : ComponentType
    {
        public const string Name = "Incinerator";

        public Incinerator()
        {
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            mResourceConsumption = new List<ResourceType>();
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Corpse>());
            addResourceProduction<Vitromeat>(); // Remove on produced
            mResourceProductionPeriod = 180f;
            mEmbeddedResourceCount = 1;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mFlags |= 1048872;
            mRadius = 2.5f;
            addUsageAnimation(CharacterAnimationType.WorkStanding);

            // TODO: decrease more morale on operating

            mName = Name;
            mIcon = ContentManager.IconIncinerator;
            mPrefabName = "PrefabMetalProcessor";

            initStrings();
        }
        public static void RegisterString()
        {
            StringUtils.RegisterString("component_incinerator", Name);
        }
    }
}
