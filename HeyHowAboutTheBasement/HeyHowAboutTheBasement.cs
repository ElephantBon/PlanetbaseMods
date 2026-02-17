using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using System.Collections.Generic;
using HeyHowAboutTheBasement.DTOs;

namespace HeyHowAboutTheBasement
{
    public class HeyHowAboutTheBasement : ModBase
    {
        public static Settings settings;

        // Parameters
        public static float processTimeFactorTunnelConstruction = 50.0f;
        
        // Variables
        public static bool creatingConnectionTunnelEntrance = false;
        private static bool guiTunnelEntranceRemoved = false;
        public static List<Module> replaceTunnelConstructions = new List<Module>();
        public static List<Module> deletingModules = new List<Module>();
        public static List<TerrainHeightData> terrainHeightDatas = new List<TerrainHeightData>();

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new HeyHowAboutTheBasement(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            ContentManager.Init(modEntry.Path);

            // Register string
            ModuleTypeMovingWalkwayController.RegisterString();
            ModuleTypeTunnelConstruction.RegisterString();
            ModuleTypeTunnelEntrance.RegisterString();
            StashMetal.RegisterString();
            StashBioplastic.RegisterString();
            StashSpares.RegisterString();
            StashMedicinalPlants.RegisterString();
            StashSemiconductors.RegisterString();

            // Register new objects
            ModuleTypeList.getInstance().AddType(new ModuleTypeMovingWalkwayController());
            ModuleTypeList.getInstance().AddType(new ModuleTypeTunnelConstruction());
            ModuleTypeList.getInstance().AddType(new ModuleTypeTunnelEntrance());
            ComponentTypeList.getInstance().AddType(new StashMetal());
            ComponentTypeList.getInstance().AddType(new StashBioplastic());
            ComponentTypeList.getInstance().AddType(new StashSpares());
            ComponentTypeList.getInstance().AddType(new StashMedicinalPlants());
            ComponentTypeList.getInstance().AddType(new StashSemiconductors());

            // Add new components to Lab
            var components = ModuleTypeList.find<ModuleTypeLab>().GetComponentTypes();
            components.Add(TypeList<ComponentType, ComponentTypeList>.find<StashMetal>());
            components.Add(TypeList<ComponentType, ComponentTypeList>.find<StashBioplastic>());
            components.Add(TypeList<ComponentType, ComponentTypeList>.find<StashSpares>());
            components.Add(TypeList<ComponentType, ComponentTypeList>.find<StashMedicinalPlants>());
            ModuleTypeList.find<ModuleTypeLab>().SetComponentTypes(components);
        }
        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Reset mod variables
            creatingConnectionTunnelEntrance = false;
            guiTunnelEntranceRemoved = false;
            replaceTunnelConstructions.Clear();
            deletingModules.Clear();
            terrainHeightDatas.Clear();
        }
        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Remove tunnel build option on GUI menu
            if(!guiTunnelEntranceRemoved)
                RemoveGuiTunnelEntrance();

            // Replace the tunnel construction site with interior building
            while(replaceTunnelConstructions.Count > 0) {
                var module = replaceTunnelConstructions[0];
                ReplaceTunnelConstructionWithEntrance(module);
                module.destroy();
                replaceTunnelConstructions.RemoveAt(0);
            }

            while(deletingModules.Count > 0) {
                deletingModules[0].destroy();
                deletingModules.RemoveAt(0);
            }

            // Restore heights after flatten
            while(terrainHeightDatas.Count > 0) {
                //Debug.Log("HeyHowAboutTheBasement: restore terrain height");

                var terrain = terrainHeightDatas[0];
                terrain.terrainData.SetHeights(terrain.num4, terrain.num5, terrain.floats);
                terrainHeightDatas.Remove(terrain);
            }
        }

        /// <summary> Remove Tunnel Entrance option from build menu </summary>
        private static void RemoveGuiTunnelEntrance()
        {
            var gameManager = GameManager.getInstance();
            if(gameManager == null)
                return;

            if(gameManager.getGameState() is GameStateGame gameState) {
                if(gameState == null)
                    return;

                var guiMenuSystem = CoreUtils.GetMember<GameStateGame, GuiMenuSystem>("mMenuSystem", gameState);
                if(guiMenuSystem == null)
                    return;

                var mMenuBuildInterior = CoreUtils.GetMember<GuiMenuSystem, GuiMenu>("mMenuBuildInterior", guiMenuSystem);
                if(mMenuBuildInterior == null)
                    return;

                var items = mMenuBuildInterior.getItems();
                if(items == null)
                    return;

                foreach(var guiItem in items) {
                    var mParameter = guiItem.getParameter();
                    if(mParameter == null)
                        continue;

                    var moduleType = mParameter as ModuleTypeTunnelEntrance;
                    if(moduleType == null)
                        continue;

                    guiTunnelEntranceRemoved = true;
                    mMenuBuildInterior.getItems().Remove(guiItem);
                    break;
                }
            }
        }

        /// <summary> Increase walk speed if the human in moving walkway </summary>
        public static float IncreaseSpeedOnWalkway(Character __instance, float speed)
        {
            var construction = __instance.getCurrentConstruction();
            if(construction != null && construction is Connection connection && connection.isEnabled()) {
                if(connection.getModule1().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule2().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule1().isEnabled()
                && connection.getModule2().isEnabled())
                    speed *= settings.MovingWalkwaySpeedMultiplier;
            }

            return speed;
        }

        public static Module ReplaceTunnelConstructionWithEntrance(Module module)
        {
            var entrance = Module.create(module.getPosition(), module.getSizeIndex(), ModuleTypeList.find<ModuleTypeTunnelEntrance>());
            entrance.onUserPlaced();
            entrance.onBuilt();
            return entrance;
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
