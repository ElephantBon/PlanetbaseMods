using Planetbase;
using PlanetbaseModUtilities;

namespace HeyHowAboutTheBasement
{
    public class StashMetal : Stash
    {
        public StashMetal()
        {
            mStoredResources.Add(TypeList<ResourceType, ResourceTypeList>.find<Metal>());
            mIcon = ResourceUtil.loadIconColor("Resources/icon_metal");
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("component_stash_metal", "Metal Stash");
        }
    }
}
