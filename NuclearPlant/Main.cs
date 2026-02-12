using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using Planetbase;
using NuclearPlant.Objects;
using UnityEngine;
using UnityModManagerNet;
using System.Linq;
using System.Collections.Generic;

namespace NuclearPlant
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Uranium ore requires technology")] public bool uraniumOreRequiresTech = false;
        [Draw("Enable hotkey")] public bool enableDebugHotkey = true;


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }

    public class Main : ModBase
    {
        public static Settings settings;
        public static System.Random random = new System.Random();
        public const double ProbabilityUraniumOre = 0.005;
        public const float ProbabilityDropsExplosion = 0.3f;

        // Parameters
        private const float craterDecayTime = 50.0f;

        // Variables
        public static List<Construction> destroyConstructions = new List<Construction>();
        public static List<Construction> damageConstructions = new List<Construction>();
        public static bool startDeletingConstructions = false;
        public static bool techNuclearPlantStateUpdated = false;
        private static Dictionary<GameObject, float> meteorCraters = new Dictionary<GameObject, float>();

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new Main(), modEntry);
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
            ContentManager.Init(modEntry.Path);

            // String
            ModuleTypeNuclearPlant.RegisterString();
            NuclearGeneratorIdle.RegisterString();
            NuclearGeneratorPowered.RegisterString();
            ResourcePower.RegisterString();
            UraniumOre.RegisterString();
            UraniumProcessor.RegisterString();
            UraniumRod.RegisterString();
            TechNuclearPlant.RegisterStrings();

            // Techs
            TechList.getInstance().AddType(new TechNuclearPlant());

            // New objects
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new UraniumOre());
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new UraniumRod());
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new ResourcePower());
            ComponentTypeList.getInstance().AddType(new NuclearGeneratorIdle());
            ComponentTypeList.getInstance().AddType(new NuclearGeneratorPowered());
            ComponentTypeList.getInstance().AddType(new UraniumProcessor());
            ModuleTypeList.getInstance().AddType(new ModuleTypeNuclearPlant());

            // Add uranium processor to factory
            var factoryComponents = ModuleTypeList.find<ModuleTypeFactory>().GetComponentTypes();
            factoryComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<UraniumProcessor>());
            ModuleTypeList.find<ModuleTypeFactory>().SetComponentTypes(factoryComponents);
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Reset mod variables
            startDeletingConstructions = false;
            techNuclearPlantStateUpdated = false;
            destroyConstructions.Clear();
            damageConstructions.Clear();
            meteorCraters.Clear();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if(!techNuclearPlantStateUpdated)
                UpdateGuiNuclearPlant();

            // Explode constructions
            if(startDeletingConstructions) {
                startDeletingConstructions = false;

                while(destroyConstructions.Count > 0) {
                    var construction = destroyConstructions[0];

                    RecycleConstruction(construction);
                    DestoryStorageResources(construction);
                    CreateMeteorCrater(construction);

                    construction.destroy();
                    destroyConstructions.RemoveAt(0);
                }


                while(damageConstructions.Count > 0) {
                    var construction = damageConstructions[0];

                    // Lockdown module
                    if(construction.getLocation() == Location.Interior)
                        construction.setLocked(true);

                    // Break roof
                    var mObjectTranslucent = CoreUtils.GetMember<Construction, GameObject>("mObjectTranslucent", construction);
                    if(mObjectTranslucent != null)
                        mObjectTranslucent.SetActive(false);

                    damageConstructions.Remove(construction);
                }

                Construction.resetGrid();
            }

            // Remove craters
            var craters = meteorCraters.Keys.ToArray();
            foreach(var crater in craters) {
                meteorCraters[crater] -= timeStep;
                if(meteorCraters[crater] <= 0) {
                    Object.Destroy(crater);
                    meteorCraters.Remove(crater);
                }
            }

            if(settings.enableDebugHotkey && Input.GetKeyUp(KeyCode.N)) {
                var plants = BuildableUtils.GetAllModules().Where(x => x.getModuleType() is ModuleTypeNuclearPlant).ToArray();
                bool somethingExploded = false;
                foreach(var plant in plants) {
                    somethingExploded = true;
                    ModuleTypeNuclearPlant.Explode(plant);
                }

                // Acquire nuclear tech if nothing exploded
                if(!somethingExploded) {
                    var tech = TypeList<Tech, TechList>.find<TechNuclearPlant>();
                    if(!Singleton<TechManager>.getInstance().isAcquired(tech)) {
                        Singleton<TechManager>.getInstance().acquire(tech);
                    }
                }
            }
        }

        internal static void ModuleExplodeParticle(Construction construction)
        {
            // Particle
            ParticleSystemData particleSystemData = Singleton<ParticleManager>.getInstance().create(ResourceList.getInstance().Particles.ShatterCorridor);
            GameObject particle = particleSystemData.getGameObject();
            particle.transform.localScale = new Vector3(construction.getRadius(), construction.getRadius(), construction.getLongRadius());
            particle.transform.position = construction.getPosition() - construction.getTransform().forward * construction.getLongRadius();
            particle.transform.rotation = construction.getTransform().rotation;
            Singleton<ParticleManager>.getInstance().autoDelete(particleSystemData);
        }

        internal static void RecycleConstruction(Construction construction)
        {
            var resourceAmounts = construction.calculateRecycleAmounts();
            if(resourceAmounts == null)
                return;

            foreach(ResourceAmount item in resourceAmounts) {
                for(int i = 0; i < item.getAmount(); i++) {
                    if(Main.random.Next(0, 100) < Main.ProbabilityDropsExplosion * 100) {
                        Resource resource = Resource.create(item.getResourceType(), construction.getPosition() + MathUtil.randFlatVector(construction.getRadius()), Location.Exterior);
                        resource.drop(Resource.State.Idle);

                        var mConditionIndicator = CoreUtils.GetMember<Resource, Indicator>("mConditionIndicator", resource);
                        mConditionIndicator.decrease(Main.random.Next(0, 80) / 100.0f);
                    }
                }
            }
        }

        internal static void DestoryStorageResources(Construction construction)
        {
            var module = construction as Module;
            if(module == null)
                return;

            var mResourceStorage = CoreUtils.GetMember<Module, ResourceStorage>("mResourceStorage", module);
            if(mResourceStorage == null)
                return;

            var slots = mResourceStorage.GetSlots();
            if(slots == null)
                return;

            foreach(var slot in slots) {
                var resources = slot.GetResources();
                if(resources == null) 
                    continue;

                for(int i = resources.Count - 1; i >= 0; i--) {
                    var resource = resources[i];
                    if(Main.random.Next(0, 100) >= Main.ProbabilityDropsExplosion * 100)
                        resource.destroy();
                    else {
                        var mConditionIndicator = CoreUtils.GetMember<Resource, Indicator>("mConditionIndicator", resource);
                        mConditionIndicator.decrease(Main.random.Next(0, 80) / 100.0f);
                    }
                }
            }
        }

        internal static void CreateMeteorCrater(Construction construction)
        {
            if(PhysicsUtil.findFloor(construction.getGameObject().transform.position, out var terrainPosition, out var normal)) {
                GameObject meteorCrater = PlanetManager.getCurrentPlanet().getDefinition().MeteorCrater;
                if(meteorCrater != null) {
                    GameObject gameObject2 = Object.Instantiate(meteorCrater);
                    gameObject2.transform.position = terrainPosition;
                    gameObject2.transform.up = normal;
                    //gameObject2.transform.SetParent(base.transform, worldPositionStays: true);
                    // How to remove the crater?
                    meteorCraters.Add(gameObject2, craterDecayTime);
                }
            }
        }

        /// <summary> Update nuclear plant availability on gui </summary>
        public static void UpdateGuiNuclearPlant()
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

                    var moduleType = mParameter as ModuleTypeNuclearPlant;
                    if(moduleType == null)
                        continue;

                    techNuclearPlantStateUpdated = true;
                    var tech = TypeList<Tech, TechList>.find<TechNuclearPlant>();
                    bool isTechAcquired = Singleton<TechManager>.getInstance().isAcquired(tech);
                    guiItem.setEnabled(isTechAcquired);

                    if(!isTechAcquired)
                        guiItem.setTooltip("Requires Tech Nuclear Power");
                    else
                        //renderBuildableTooltip(x, y, tooltip.Substring("(Build)".Length));
                        guiItem.setTooltip("(Build)ModuleTypeNuclearPlant");
                    break;
                }
            }
        }
    }
}
