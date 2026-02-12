using GeniusEngineers.Objects;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager;


namespace GeniusEngineers
{
    public class Main : ModBase
    {
        // Researchs 
        public enum Research { ExtraBotLife, ExtraVegetableLife, ExtraPowerStorage, ExtraWaterStorage }
        public static float ExtraBotLife { get; set; }
        public static float ExtraVegetableLife { get; set; }
        public static float ExtraPowerStorage { get; set; }
        public static float ExtraWaterStorage { get; set; }

        public static new void Init(ModEntry modEntry)
        {
            InitializeMod(new Main(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            // Register strings
            ResearchTable.RegisterString();
            ResourceExtraBotLife.RegisterString();
            ResourceExtraVegetableLife.RegisterString();
            ResourceExtraPowerStorage.RegisterString();
            ResourceExtraWaterStorage.RegisterString();

            // Add new objects
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new ResourceExtraBotLife());
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new ResourceExtraVegetableLife());
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new ResourceExtraPowerStorage());
            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new ResourceExtraWaterStorage());
            ComponentTypeList.getInstance().AddType(new ResearchTable());

            // Add new components to module
            var components = ModuleTypeList.find<ModuleTypeLab>().GetComponentTypes();
            components.Add(TypeList<ComponentType, ComponentTypeList>.find<ResearchTable>());
            ModuleTypeList.find<ModuleTypeLab>().SetComponentTypes(components);
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Reset mod variables
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
#if DEBUG
            if(Input.GetKeyUp(KeyCode.U)) {
                CompleteResearch(Research.ExtraPowerStorage);
            }
            else
            if(Input.GetKeyUp(KeyCode.B)) {
                var mModules = CoreUtils.GetMember<Module, List<Module>>("mModules");
                foreach(var module in mModules) {
                    if(module.getModuleType() is ModuleTypeLandingPad) {
                        Character.create(SpecializationList.find<Carrier>(), module.getPosition(), module.getLocation());
                        break;
                    }
                }
            }
            else
            if(Input.GetKeyUp(KeyCode.M)) {
                LandingShipManager.getInstance().trySpawnMerchantShip();
            }
#endif
        }

        public static void CompleteResearch(Research research)
        {
            const float adv = 0.1f;
            switch(research) {
                case Research.ExtraBotLife: {
                    ExtraBotLife += adv;

                    // Update performance of existing bots
                    var mCharacters = CoreUtils.GetMember<Character, List<Character>>("mCharacters");
                    foreach(var character in mCharacters) {
                        if(character is Bot bot) {
                            var mIntegrityDecayRate = CoreUtils.GetMember<Bot, float>("mIntegrityDecayRate", bot);
                            CoreUtils.SetMember("mIntegrityDecayRate", bot, mIntegrityDecayRate * (100 + adv) / 100);
                        }
                    }
                } break;
                case Research.ExtraVegetableLife: {
                    ExtraVegetableLife += adv;

                    // Update condition of existing pads
                    var mComponents = CoreUtils.GetMember<ConstructionComponent, List<ConstructionComponent>>("mComponents");
                    foreach(var component in mComponents) {
                        if(component.getComponentType() is VegetablePad pad) {
                            var mConditionDecayTime = CoreUtils.GetMember<VegetablePad, float>("mConditionDecayTime", pad);
                            CoreUtils.SetMember("mConditionDecayTime", pad, mConditionDecayTime * (100 + adv) / 100);
                        }
                    }
                } break;
                case Research.ExtraPowerStorage: {
                    ExtraPowerStorage += adv;

                    // Update storage of existing power collectors
                    var mModules = CoreUtils.GetMember<Module, List<Module>>("mModules");
                    foreach(var module in mModules) {
                        if(module.getModuleType() is ModuleTypePowerCollector moduleType) {
                            var indicator = module.getPowerStorageIndicator();
                            indicator.setMax(moduleType.getPowerStorageCapacity(module.getSizeFactor()));
                        }
                    }
                } break;
                case Research.ExtraWaterStorage: {
                    ExtraWaterStorage += adv;

                    // Update storage of existing water tanks
                    var mModules = CoreUtils.GetMember<Module, List<Module>>("mModules");
                    foreach(var module in mModules) {
                        if(module.getModuleType() is ModuleTypeWaterTank moduleType) {
                            var indicator = module.getWaterStorageIndicator();
                            indicator.setMax(moduleType.getWaterStorageCapacity(module.getSizeFactor()));
                        }
                    }
                } break;
            }
        }
    }
}
