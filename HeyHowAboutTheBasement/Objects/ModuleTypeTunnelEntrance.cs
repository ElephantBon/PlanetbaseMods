using Planetbase;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement
{
    public class ModuleTypeTunnelEntrance : ModuleType
    {
        public const string Name = "Tunnel Entrance";

        public ModuleTypeTunnelEntrance()
        {
            mPowerGeneration = -1000;
            mMinSize = 1;
            mMaxSize = 1;
            mLayoutType = LayoutType.Cross;
            mFlags = FlagNoFoundations | FlagWalkable;

            mName = Name;
            mIcon = ContentManager.IconTunnel;
            mModels[1] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabMine2");

            initStrings();
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("tunnel_entrance", Name);
        }
    }
}
