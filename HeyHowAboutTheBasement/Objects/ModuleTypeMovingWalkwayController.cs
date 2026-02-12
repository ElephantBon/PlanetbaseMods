using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement
{
    public class ModuleTypeMovingWalkwayController : ModuleType
    {
        public const string Name = "Moving Walkway Controller";

        public ModuleTypeMovingWalkwayController()
        {
            mPowerGeneration = -3000;
            mMinSize = 0;
            mMaxSize = 0;
            mLayoutType = LayoutType.Normal;
            mFlags = 1050672;
            mRequiredStructure.set<ModuleTypeOxygenGenerator>();

            mCost = new ResourceAmounts();
#if DEBUG
            mCost.add(ResourceTypeList.MetalInstance, 1);
#else
            mCost.add(ResourceTypeList.MetalInstance, 1);
            mCost.add(ResourceTypeList.SparesInstance, 2);
#endif

            mIcon = ContentManager.IconMovingWalkway;
            mModels[0] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabLab1");

            initStrings();
            mName = Name;
            mTooltip = "Control moving walkway for long distance traveling.";
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("moving_walkway_controller", Name);
        }
    }
}
