using Planetbase;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;
using StorageGuru.Patches;

namespace StorageGuru
{
    public class StorageGuru : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new StorageGuru(), modEntry);

        public const bool UseImprovedSlotLayout = true;
        public static List<ResourceType> MasterResourceDefinitions { get; private set; }

        public override void OnInitialized(ModEntry modEntry)
        {
            ContentManager.Init(modEntry.Path);

            Debug.Log("[MOD] Storage Guru activated");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            MenuController.Update();
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            RefreshResourceDefinitions();
            ContentManager.CreateAlternativeIcons(MasterResourceDefinitions);
        }

        private void RefreshResourceDefinitions()
        {
            MasterResourceDefinitions = TypeList<ResourceType, ResourceTypeList>.get().Where(x => IsValidResourceType(x)).ToList();  // Clear out non-storeable resources (coins)            
        }

        private bool IsValidResourceType(ResourceType type)
        {
            if(type is Coins)
                return false;
            if(WhereTheDeadBodiesPatch.IsLoaded()) {
                if(WhereTheDeadBodiesPatch.IsCorpse(type))
                    return false;
                if(WhereTheDeadBodiesPatch.IsRemains(type))
                    return false;
            }
            return true;
        }

        public static void Log(string message)
        {
            Debug.Log($"[Mod] Storage Guru: {message}");
        }

        #region Character Targeting 

        public Module FindNearestModule(Vector3 position, List<Module> modules)
        {
            try
            {
                if (modules != null && modules.Count > 0 && position != null)
                {
                    var orderedModules = modules.OrderBy(x => x.distanceTo(position)).ToList();

                    if (orderedModules != null && orderedModules.Count > 0)
                    {
                        return orderedModules[0];
                    }
                }
            }
            catch (NullReferenceException)
            {
            }

            return null;
        }

        #endregion
    }
}
