using HarshWorld.Disasters.RogueBot;
using HarshWorld.Patches;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace HarshWorld
{
    public class HarshWorld : ModBase
    {
        public static Settings settings;
        public const float RealTimeFactor = 2f;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new HarshWorld(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            AssetManager.Init(modEntry.Path);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if (GetGameStateGame() == null)
                return;

            CustomDisasterManager.getInstance().update(timeStep);

            if (Input.GetKeyUp(settings.KeyDisasterMenu))
            {
                GuiMenuSystemPatch.onOpenDisasterMenu();
            }

            //if (Input.GetKeyUp(settings.KeyTest))
            //{
            //    var selected = Selection.getSelected();
            //    if(selected != null && selected is Bot)
            //    {
            //        RogueBot.goRogue(selected as Bot);
            //    }
            //}
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
