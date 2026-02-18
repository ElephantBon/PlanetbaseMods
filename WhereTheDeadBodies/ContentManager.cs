using AssetUtility;
using System.IO;
using UnityEngine;

namespace WhereTheDeadBodies
{
    internal class ContentManager
    {
        public static Texture2D IconIncinerator { get; private set; }
        public static Texture2D IconCorpse { get; private set; }
        public static Texture2D IconRemains { get; private set; }
        public static Texture2D IconMorgue { get; private set; }
        public static GameObject PrefabBloodSplatter { get; private set; }

        public static void Init(string modPath)
        {
            // Green
            IconIncinerator = AssetUtils.LoadTextureColorDefault(Path.Combine(modPath, @"Assets\icon_incinerator.png"));
            IconMorgue = AssetUtils.LoadTextureColorDefault(Path.Combine(modPath, @"Assets\icon_morgue.png"));

            // Colorful
            IconCorpse = AssetUtils.LoadTexture(Path.Combine(modPath, @"Assets\icon_corpse.png"));
            IconRemains = AssetUtils.LoadTexture(Path.Combine(modPath, @"Assets\icon_remains.png"));

            PrefabBloodSplatter = AssetUtils.LoadGameObject(Path.Combine(modPath, @"Assets\wherethedeadbodies"), "PrefabBloodSplatter");
        }
    }
}
