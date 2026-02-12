using AssetUtility;
using System.IO;
using UnityEngine;

namespace NuclearPlant
{
    internal class ContentManager
    {
        public static Texture2D IconNuclearPlant { get; private set; }
        public static Texture2D IconTechNuclearPlant { get; private set; }
        public static Texture2D IconPower { get; private set; }
        public static Texture2D IconResourcePower { get; private set; }
        public static Texture2D IconUraniumOre { get; private set; }
        public static Texture2D IconUraniumProcessor { get; private set; }
        public static Texture2D IconUraniumRod { get; private set; }

        public static void Init(string modPath)
        {
            // Gui Green
            IconNuclearPlant = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\icon_nuclear_plant.png"));
            IconPower = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\icon_power.png"));
            IconUraniumProcessor = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\icon_uranium_processor.png"));

            // Colorless
            IconResourcePower = AssetUtils.LoadTexture(Path.Combine(modPath, @"Assets\icon_power.png"));
            IconUraniumOre = AssetUtils.LoadTexture(Path.Combine(modPath, @"Assets\icon_uranium_ore.png"));
            IconUraniumRod = AssetUtils.LoadTexture(Path.Combine(modPath, @"Assets\icon_uranium_rod.png"));
            IconTechNuclearPlant = AssetUtils.LoadTexture(Path.Combine(modPath, @"Assets\icon_tech_nulear_plant.png"));
        }
    }
}
