using Planetbase;
using UnityEngine;
using PlanetbaseModUtilities;

namespace GeniusEngineers
{
    internal class ResourceExtraVegetableLife : ResourceType
    {
        public const string Name = "Extra Vegetable Life";

        public ResourceExtraVegetableLife()
        {
            mStatsColor = new Color32(255, 255, 0, byte.MaxValue);
            mFlags = 2; // Coins
            mValue = 1;
            mMerchantCategory = MerchantCategory.Count;

            mModel = null;
            mIcon = Util.applyColor(ResourceUtil.loadIcon("Resources/icon_vegetables"));
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("resource_extra_vegetable_life", Name);
        }
    }
}
