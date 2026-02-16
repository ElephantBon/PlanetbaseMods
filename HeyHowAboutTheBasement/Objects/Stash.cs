using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyHowAboutTheBasement
{
    public class Stash : ComponentType
    {
        public Stash()
        {
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(ResourceTypeList.MetalInstance, 1);
            mEmbeddedResourceCount = 10;
            mStoredResources = new List<ResourceType>();
            mFlags = 256;

            mPrefabName = "PrefabArmory";

            initStrings();
        }
    }
}
