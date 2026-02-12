using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using System.Collections.Generic;
using HeyHowAboutTheBasement.DTOs;

namespace HeyHowAboutTheBasement
{
    public class Main : ModBase
    {
        public static Settings settings;

        // Parameters
        public static float processTimeFactorTunnelConstruction =
#if DEBUG
            0.5f;
#else
            50.0f;
#endif

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
            InitializeMod(new Main(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            ContentManager.Init(modEntry.Path);

            // Register string
            ModuleTypeMovingWalkwayController.RegisterString();
            ModuleTypeTunnelConstruction.RegisterString();
            ModuleTypeTunnelEntrance.RegisterString();

            // Register new objects
            ModuleTypeList.getInstance().AddType(new ModuleTypeMovingWalkwayController());
            ModuleTypeList.getInstance().AddType(new ModuleTypeTunnelConstruction());
            ModuleTypeList.getInstance().AddType(new ModuleTypeTunnelEntrance());
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
            if(((Settings)settings).NoTimeToExplain)
                return speed * 200f;

            var construction = __instance.getCurrentConstruction();
            if(construction != null && construction is Connection connection && connection.isEnabled()) {
                if(connection.getModule1().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule2().getModuleType() is ModuleTypeMovingWalkwayController
                && connection.getModule1().isEnabled()
                && connection.getModule2().isEnabled())
                    speed *= 4f;
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
