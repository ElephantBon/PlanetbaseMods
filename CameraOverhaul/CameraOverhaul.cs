using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace CameraOverhaul
{
    public class CameraOverhaul : ModBase
    {
        public static float mVerticalRotationAcceleration = 0f;
        public static float mPreviousMouseY = 0f;

        public static float mAlternateRotationAcceleration = 0f;

        public static float TerrainTotalSize => CoreUtils.GetMember<float>("TotalSize", typeof(TerrainGenerator));
        public static Plane mGroundPlane = new Plane(Vector3.up, new Vector3(TerrainTotalSize, 0f, TerrainTotalSize) * 0.5f);

        public static int mModulesize = 0;
        public static bool mIsPlacingModule = false;

        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new CameraOverhaul(), modEntry);
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing needed here
        }
    }
}
