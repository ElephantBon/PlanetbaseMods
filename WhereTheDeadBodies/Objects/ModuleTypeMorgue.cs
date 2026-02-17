using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;

namespace WhereTheDeadBodies.Objects
{
    public class ModuleTypeMorgue : ModuleType
    {
        public const string Name = "Morgue";

        public ModuleTypeMorgue()
        {
            mPowerGeneration = -1000;
            mMinSize = 1;
            mMaxSize = 1;
            mFlags = 56;
            mLayoutType = LayoutType.Circular;
            mRequiredStructure.set<ModuleTypeOxygenGenerator>();

            mComponentTypes = new ComponentType[]
            {
                TypeList<ComponentType, ComponentTypeList>.find<Incinerator>(),
            };

            mName = Name;
            mIcon = ContentManager.IconMorgue;// ResourceUtil.loadIconColor("Modules/icon_dorm");
            mModels[1] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabStorage2");

            initStrings();
        }
        public static void RegisterString()
        {
            StringUtils.RegisterString("morgue", Name);
        }
    }
}
