using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Planetbase.Human;
using PlanetbaseModUtilities;
using System.Xml.Linq;

namespace NuclearPlant
{
    internal class UraniumRod : ResourceType
    {
        public const string Name = "Uranium Rod";

        public UraniumRod()
        {
            mStatsColor = new Color32(128, 255, 0, byte.MaxValue);
            mValue = 120;
            mMerchantCategory = MerchantCategory.Electronics;
            mSize = ResourceType.LargeResourceSize;

            mModel = ResourceUtil.loadPrefab("Prefabs/Resources/PrefabMetal");
            mIcon = Util.applyColor(ContentManager.IconUraniumRod, mStatsColor);
            mName = Name;
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("uranium_rod", Name);
        }
    }
}
