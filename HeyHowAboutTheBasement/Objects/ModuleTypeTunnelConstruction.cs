using Planetbase;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement
{
    public class ModuleTypeTunnelConstruction : ModuleType
    {
        public const string Name = "Tunnel";

        public ModuleTypeTunnelConstruction()
        {
            mPowerGeneration = -3000;
            mExterior = true;
            mMinSize = 1;
            mMaxSize = 1;
            mMaxUsers = 3;
            mFlags = 1048850;
            mRequiredStructure.set<ModuleTypeSolarPanel>();
            mExteriorNavRadius = Module.ValidSizes[2] * 0.5f;

            mCost = new ResourceAmounts();
#if DEBUG
            mCost.add(ResourceTypeList.MetalInstance, 1);
#else
            mCost.add(ResourceTypeList.MetalInstance, 20);
#endif

            mCondicionDecayTime = 100f;
            mRepairResource = TypeList<ResourceType, ResourceTypeList>.find<Metal>();

            mName = Name;
            mIcon = ContentManager.IconTunnel; //ResourceUtil.loadIconColor("Modules/icon_mine");
            mModels[1] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabMine2");

            initStrings();
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("tunnel_construction", Name);
        }
    }
}
