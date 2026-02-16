using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyHowAboutTheBasement
{
    public class StashSemiconductors : Stash
    {
        public StashSemiconductors()
        {
            mStoredResources.Add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>());
            mIcon = ResourceUtil.loadIconColor("Resources/icon_semiconductors");
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("component_stash_semiconductors", "Semiconductors Stash");
        }
    }
}
