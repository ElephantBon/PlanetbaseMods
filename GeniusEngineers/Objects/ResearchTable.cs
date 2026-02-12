using Planetbase;
using System.Collections.Generic;
using PlanetbaseModUtilities;

namespace GeniusEngineers.Objects
{
    public class ResearchTable : ComponentType
    {
        public const string Name = "Research Table";

        public ResearchTable()
        {
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);
            mResourceConsumption = new List<ResourceType>();
            mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>());

            // The order must be same as GeniusEngineers.Research
            addResourceProduction<ResourceExtraBotLife>();
            addResourceProduction<ResourceExtraVegetableLife>();
            addResourceProduction<ResourceExtraPowerStorage>();
            addResourceProduction<ResourceExtraWaterStorage>();

#if DEBUG
            mResourceProductionPeriod = 10f;
#else
            mResourceProductionPeriod = 600f;
#endif

            mEmbeddedResourceCount = 2;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Engineer>();
            addUsageAnimation(CharacterAnimationType.WorkSeated);
            mFlags |= 1048872 | FlagAlternativeProduction;
            mRadius = 1.25f;

            mPrefabName = "PrefabWorkbench";
            mIcon = ResourceList.StaticIcons.Tech;
            initStrings();

            mName = Name;
            mTooltip = "A place for Engineer to research base improvements.";
        }
        public static void RegisterString()
        {
            StringUtils.RegisterString("component_research_table", Name);
        }
    }
}
