using System.IO;
using UnityEngine;
using AssetUtility;

namespace HeyHowAboutTheBasement
{
    public class ContentManager
    {
        public static Texture2D IconTunnel { get; private set; }
        public static Texture2D IconMovingWalkway { get; private set; }
        public static GameObject PrefabMovingWalkwayController { get; private set; }

        public static void Init(string modPath)
        {
            IconTunnel = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\icon_tunnel.png"));
            IconMovingWalkway = AssetUtils.LoadTextureColored(Path.Combine(modPath, @"Assets\icon_moving_walkway.png"));

            PrefabMovingWalkwayController = AssetUtils.LoadGameObject(Path.Combine(modPath, @"Assets\HeyHowAboutTheBasement.assetbundle"), "Walkway Controller");
            PrefabMovingWalkwayController.transform.localScale *= 15.0f;
        }
    }

}
