using Planetbase;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement
{
    public class StashBioplastic : Stash
    {
        public StashBioplastic()
        {
            mStoredResources.Add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>());
            mIcon = ResourceUtil.loadIconColor("Resources/icon_bioplastic");
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("component_stash_bioplastic", "Bioplastic Stash");
        }
    }
}
