using AssetUtility;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace StorageGuru
{
    internal static class ContentManager
    {
        public static Texture2D EnableAllIcon { get; private set; }
        public static Texture2D DisableAllIcon { get; private set; }

        private const float greyscaleBrightness = 180f / 255f;

        public static Dictionary<string, Texture2D> GreyscaleTextures; 

        public static void Init(string modPath)
        {
            StringUtils.RegisterString("tooltip_manage_storage", "Manage Storage");
            GreyscaleTextures = new Dictionary<string, Texture2D>();

            EnableAllIcon = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\StorageEnable.png"));
            DisableAllIcon = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\StorageDisable.png"));
        }

        public static void CreateAlternativeIcons(List<ResourceType> resourceTypes)
        {
            GreyscaleTextures = new Dictionary<string, Texture2D>(); 

            foreach (var resourceType in resourceTypes)
            {
                GreyscaleTextures.Add(resourceType.getName(), ApplyGreyscaleColorFix(resourceType.getIcon())); 
            }
        }

        private static Texture2D ApplyGreyscaleColorFix(Texture2D texture)
        {
            var pixels = texture.GetPixels();
            var greyscalePixels = pixels.Select(p => new Color(greyscaleBrightness, greyscaleBrightness, greyscaleBrightness, p.a)).ToArray();
            texture.SetPixels(greyscalePixels);
            return Util.applyColor(texture); // Apply standard Gui color
        }
    }
}
