using Planetbase;
using UnityEngine;
using PlanetbaseModUtilities;

namespace GeniusEngineers
{
    internal class ResourceExtraBotLife : ResourceType
    {
        public const string Name = "Extra Bot Life";

        public ResourceExtraBotLife()
        {
            mStatsColor = new Color32(255, 255, 0, byte.MaxValue);
            mFlags = 2; // Coins
            mValue = 1;
            mMerchantCategory = MerchantCategory.Count;

            mModel = null;
            mIcon = ResourceList.StaticIcons.Bot;
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("resource_extra_bot_life", Name);
        }
    }
}
