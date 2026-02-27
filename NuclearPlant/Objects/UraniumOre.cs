using Planetbase;
using UnityEngine;
using PlanetbaseModUtilities;

namespace NuclearPlant.Objects
{
    internal class UraniumOre : ResourceType
    {
        public const string Name = "Uranium Ore";

        public UraniumOre()
        {
            mStatsColor = new Color32(0, 255, 0, byte.MaxValue);
            mValue = 150;
            mMerchantCategory = MerchantCategory.RawMaterial;
            mSize = ResourceType.LargeResourceSize;

            mModel = ResourceUtil.loadPrefab("Prefabs/Resources/PrefabOre");
            mIcon = Util.applyColor(ContentManager.IconUraniumOre, mStatsColor);
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("uranium_ore", Name);
        }
    }
}
