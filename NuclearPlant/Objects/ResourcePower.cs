using Planetbase;
using UnityEngine;
using PlanetbaseModUtilities;

namespace NuclearPlant.Objects
{
    internal class ResourcePower : ResourceType
    {
        public const string Name = "Power";

        public ResourcePower()
        {
            mStatsColor = new Color32(255, 255, 0, byte.MaxValue);
            mValue = 10;
            mMerchantCategory = MerchantCategory.Count;
            mSize = ResourceType.LargeResourceSize;

            mModel = ResourceUtil.loadPrefab("Prefabs/Resources/PrefabOre");
            mIcon = Util.applyColor(ContentManager.IconResourcePower, mStatsColor);
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("resource_power", Name);
        }
    }
}
