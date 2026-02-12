using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using Planetbase;
using NuclearPlant.Objects;
using UnityEngine;
using UnityModManagerNet;
using System.Linq;
using System.Collections.Generic;

namespace NuclearPlant
{
    public class TechNuclearPlant : Tech
    {
        public TechNuclearPlant()
        {
            load();
            this.mMerchantCategory = MerchantCategory.Electronics;
            this.mValue = 1800;
        }

        public new void load()
        {
            this.initStrings();
            this.mIcon = Util.applyColor(ContentManager.IconTechNuclearPlant, Color.cyan); 
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("tech_nuclear_plant", "Nuclear Power");
        }
    }
}
