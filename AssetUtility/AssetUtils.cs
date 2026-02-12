using Planetbase;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace AssetUtility
{
    /// <summary>
    /// Require UnityEngine.AssetBundleModule.dll and UnityEngine.ImageConversionModule.dll
    /// </summary>
    public static class AssetUtils
    {
        private static Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>();

        private static AssetBundle LoadAssetBundle(string path)
        {
            if(assetBundleCache.ContainsKey(path))
                return assetBundleCache[path];
            else {
                var assetBundle = AssetBundle.LoadFromFile(path);
                if(assetBundle != null)
                    assetBundleCache[path] = assetBundle;
                return assetBundle;
            }
        }

        public static GameObject LoadGameObject(string assetPath, string objectName)
        {
            return LoadAssetBundle(assetPath).LoadAsset<GameObject>(objectName);
        }

        public static Texture2D LoadTexture(string path)
        {
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(File.ReadAllBytes(path));
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        /// <summary>
        /// To be deprecated
        /// </summary>
        public static Texture2D LoadTextureColored(string path)
        {
            return Util.applyColor(LoadTexture(path));
        }

        public static Texture2D LoadTextureColorDefault(string path)
        {
            return Util.applyColor(LoadTexture(path));
        }

        public static Texture2D LoadTextureColor(string path, Color color)
        {
            return Util.applyColor(LoadTexture(path), color);
        }

    }
}

