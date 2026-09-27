using AssetUtility;
using System.IO;
using UnityEngine;

namespace HarshWorld
{
    internal class AssetManager
    {
        public static Texture2D IconFire { get; private set; }
        public static Texture2D IconLock { get; private set; }
        public static Texture2D IconUnlock { get; private set; }

        public static void Init(string modPath)
        {
            // Green
            IconFire = AssetUtils.LoadTextureColorDefault(Path.Combine(modPath, @"Assets\icon_fire.png"));
            IconLock = AssetUtils.LoadTextureColorDefault(Path.Combine(modPath, @"Assets\icon_lock.png"));
            IconUnlock = AssetUtils.LoadTextureColorDefault(Path.Combine(modPath, @"Assets\icon_unlock.png"));
        }
    }
}
