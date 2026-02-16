using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace FreeBuilding
{
    public class FreeBuilding : ModBase
    {
        public static Settings settings;

        // Variables
        public static bool freeModeEnabled = false;
        public static bool placingBeyondBorder = false;
        private static int rotationFacingIndex = 0;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new FreeBuilding(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if(Input.GetKeyUp(settings.KeyToggleFreeMode))
                freeModeEnabled = !freeModeEnabled;

            //allows module rotaion pre-placement
            var gameState = GameStateUtils.GetGameStateGameUpdating();
            if(gameState != null) { 
                var activeModule = CoreUtils.GetMember<GameStateGame, Module>("mActiveModule", gameState);
                if(activeModule == null)
                    return;

                var mMode = CoreUtils.GetMember<GameStateGame, int>("mMode", gameState);
                if(mMode != 1) // GameStateGame.Mode.PlacingModule
                    return;

                #region Auto Rotate Building
                List<Vector3> connectionPositions = new List<Vector3>();
                var mConstructions = CoreUtils.GetMember<Construction, List<Construction>>("mConstructions");
                for(int i = 0; i < mConstructions.Count; ++i) {
                    if(mConstructions[i] is Module module && module != activeModule && Connection.canLink(activeModule, module)) {
                        connectionPositions.Add(module.getPosition());
                    }
                }

                if(connectionPositions.Count == 0) 
                    return;                

                // If there are more than 1 connections, press the rotation key to cycle through them
                rotationFacingIndex = Math.Min(rotationFacingIndex, connectionPositions.Count - 1);
                if(Input.GetKeyUp(settings.KeyRotation)) {
                    rotationFacingIndex = ++rotationFacingIndex % connectionPositions.Count;
                }

                var mObject = CoreUtils.GetMember<Construction, GameObject>("mObject", activeModule);
                mObject.transform.localRotation = Quaternion.LookRotation((connectionPositions[rotationFacingIndex] - activeModule.getPosition()).normalized);
                #endregion
            }
        }

        static void OnGUI(ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }
}
