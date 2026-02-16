using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
