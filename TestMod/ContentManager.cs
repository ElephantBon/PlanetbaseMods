

using Planetbase;
using System.IO;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace TestMod
{
    internal class ContentManager
    {
        public static Texture2D IconTunnel { get; private set; }
        public static GameObject PrefabTunnel { get; private set; }

        public static void Load()
        {
            // Load assets from asset bundle
            var assets = AssetBundle.LoadFromFile(Path.Combine(Main.mod.Path, "Assets\\firevfx.assets"));
            PrefabTunnel = assets.LoadAsset<GameObject>("VFX_Fire_01_Big");

            // Load texture from image file
            IconTunnel = LoadFromImageFile(Path.Combine(Main.mod.Path, "Assets\\icon_tunnel.png"));
        }

        private static Texture2D LoadFromImageFile(string filepath)
        {
            if(File.Exists(filepath)) {
                Main.mod.Logger.Log("Loading icon from " + filepath);
                //IconTunnel = ResourceUtil.loadIcon(filepath); // This is no longer working in newer Unity version
                var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                tex.LoadImage(File.ReadAllBytes(filepath));
                tex.wrapMode = TextureWrapMode.Clamp;
                return Util.applyColor(tex);
            }
            else {
                Main.mod.Logger.Log("File not found: " + filepath);
                return null;
            }
        }
    }
}
