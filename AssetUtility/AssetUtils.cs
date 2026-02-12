using Planetbase;
using System.IO;
using UnityEngine;


namespace AssetUtility
{
    /// <summary>
    /// Require UnityEngine.AssetBundleModule.dll and UnityEngine.ImageConversionModule.dll
    /// </summary>
    public static class AssetUtils
    {
        public static AssetBundle LoadAssetBundle(string path)
        {
            return AssetBundle.LoadFromFile(path);
        }

        public static GameObject LoadGameObject(AssetBundle assetBundle, string path)
        {
            return assetBundle.LoadAsset<GameObject>(path);
        }

        public static Texture2D LoadTexture(string path)
        {
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(File.ReadAllBytes(path));
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        public static Texture2D LoadTextureColored(string path)
        {
            return Util.applyColor(LoadTexture(path));
        }
    }
}

