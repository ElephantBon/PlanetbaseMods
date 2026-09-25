using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace HarshWorld
{
    public class Main : ModBase
    {
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new Main(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if(Input.GetKeyUp(settings.KeyTest))
            {
                MeteorRainController.GetOrCreate().StartRain();
            }
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }
}
