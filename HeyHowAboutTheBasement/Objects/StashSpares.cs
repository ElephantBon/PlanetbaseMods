using Planetbase;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement
{
    public class StashSpares : Stash
    {
        public StashSpares()
        {
            mStoredResources.Add(TypeList<ResourceType, ResourceTypeList>.find<Spares>());
            mIcon = ResourceUtil.loadIconColor("Resources/icon_spares");
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("component_stash_spares", "Spares Stash");
        }
    }
}
