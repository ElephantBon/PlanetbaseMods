using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace WhereTheDeadBodies.Objects
{
    internal class Remains : ResourceType
    {
        public const string Name = "Remains";

        public Remains()
        {
            mStatsColor = new Color32(255, 255, 255, byte.MaxValue);
            mValue = 1;
            mMerchantCategory = MerchantCategory.Count;
            mSize = ResourceType.LargeResourceSize;

            mName = Name;
            mIcon = Util.applyColor(ContentManager.IconRemains, mStatsColor);
            mModel = ResourceUtil.loadPrefab("Prefabs/Resources/PrefabMetal");
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("remains", Name);
        }
    }
}
