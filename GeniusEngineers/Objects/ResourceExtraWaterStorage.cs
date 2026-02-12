using Planetbase;
using UnityEngine;
using PlanetbaseModUtilities;

namespace GeniusEngineers
{
    internal class ResourceExtraWaterStorage: ResourceType
    {
        public const string Name = "Extra Water Storage";

        public ResourceExtraWaterStorage()
        {
            mStatsColor = new Color32(255, 255, 0, byte.MaxValue);
            mFlags = 2; // Coins
            mValue = 1;
            mMerchantCategory = MerchantCategory.Count;

            mModel = null;
            mIcon = ResourceList.StaticIcons.Water;
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("resource_extra_water_storage", Name);
        }
    }
}
