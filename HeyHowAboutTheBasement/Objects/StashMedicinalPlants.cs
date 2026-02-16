using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyHowAboutTheBasement
{
    public class StashMedicinalPlants : Stash
    {
        public StashMedicinalPlants()
        {
            mStoredResources.Add(TypeList<ResourceType, ResourceTypeList>.find<MedicinalPlants>());
            mIcon = ResourceUtil.loadIconColor("Resources/icon_medicinal_plants");
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("component_stash_medicinal_plants", "Medicinal Plants Stash");
        }
    }
}
