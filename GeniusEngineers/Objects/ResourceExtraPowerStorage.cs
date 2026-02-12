using Planetbase;
using UnityEngine;
using PlanetbaseModUtilities;

namespace GeniusEngineers
{
    internal class ResourceExtraPowerStorage : ResourceType
    {
        public const string Name = "Extra Power Storage";

        public ResourceExtraPowerStorage()
        {
            mStatsColor = new Color32(255, 255, 0, byte.MaxValue);
            mFlags = 2; // Coins
            mValue = 1;
            mMerchantCategory = MerchantCategory.Count;

            mModel = null;
            mIcon = ResourceList.StaticIcons.PowerStorage;
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("resource_extra_power_storage", Name);
        }
    }
}
