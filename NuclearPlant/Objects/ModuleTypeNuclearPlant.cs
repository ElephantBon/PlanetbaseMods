using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearPlant.Objects
{
    public class ModuleTypeNuclearPlant : ModuleType
    {
        public const string Name = "Nuclear Plant";

        public ModuleTypeNuclearPlant()
        {
            mPowerGeneration = -1000;
            mMinSize = 1;
            mMaxSize = 1;
            mLayoutType = LayoutType.Circular;
            mCondicionDecayTime = 24000;

            mRequiredStructure.set<ModuleTypeFactory>();

#if !DEBUG
            mCost = new ResourceAmounts();
            mCost.add(ResourceTypeList.MetalInstance, 10);
            mCost.add(ResourceTypeList.BioplasticInstance, 10);
            mCost.add(ResourceTypeList.find<Semiconductors>(), 4);
#endif

            mComponentTypes = new ComponentType[]
            {
                TypeList<ComponentType, ComponentTypeList>.find<NuclearGeneratorIdle>(),
            };

            mIcon = ContentManager.IconNuclearPlant;
            mModels[1] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabOxygenGenerator2");
            mModels[1].setColor(Color.red);
            mName = Name;

            initStrings();
        }

        public static void RegisterString()
        {
            StringUtils.RegisterString("nuclear_plant", Name);
        }


        internal static void Explode(Module module)
        {
            // Influence range 1 : modules destroyed in 100%
            var radius1 = 50;

            // Influence range 2 : modules/components damaged
            var radius2 = 120;
            
            // Influence range 3 : modules/components damaged, colonists suffer in radiation
            var radius3 = 300;


            var centerPosition = module.getPosition();

            RaycastHit[] array2 = Physics.SphereCastAll(centerPosition + Vector3.up * 20f, radius3, Vector3.down, 40f, 4198400);
            if(array2 != null) {
                var mConstructionDictionary = CoreUtils.GetMember<Construction, Dictionary<GameObject, Construction>>("mConstructionDictionary");
                var mCharacters = CoreUtils.GetMember<Character, List<Character>>("mCharacters");
                var destroyComponents = new List<ConstructionComponent>();
                var conditionTypeRadiation = TypeList<ConditionType, ConditionTypeList>.find<ConditionRadiation>();
                for(int k = 0; k < array2.Length; k++) {
                    RaycastHit raycastHit = array2[k];
                    GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    Construction construction = mConstructionDictionary[gameObject];

                    if(construction == null)
                        continue;

                    float distToConstruction = (centerPosition - construction.getPosition()).magnitude;


                    if(distToConstruction <= radius3) {
                        // Damage poeple
                        foreach(var character in mCharacters) {
                            if(character.getCurrentConstruction() == construction) {
                                var damage = distToConstruction / radius3 + Random.Range(0, 25) / 100.0f;
                                character.decayIndicator(CharacterIndicator.Health, damage);
                                character.decayIndicator(CharacterIndicator.Condition, damage);

                                // Set radiation
                                if(!(character is Bot)) {
                                    Condition condition = character.getCondition();
                                    if(condition == null || condition.getConditionType() != conditionTypeRadiation)
                                        character.setCondition(conditionTypeRadiation);
                                }
                            }
                        }
                    }

                    if(distToConstruction <= radius1) {
                        // Destroy module
                        if(!(construction is Connection))
                            Main.destroyConstructions.Add(construction);

                        // Particle
                        Main.ModuleExplodeParticle(construction);

                        // Kill people inside
                        foreach(var character in mCharacters)
                            if(character.getCurrentConstruction() == construction) {
                                character.decayIndicator(CharacterIndicator.Health, 1f);
                                character.decayIndicator(CharacterIndicator.Condition, 1f);
                            }
                    }
                    else 
                     if(distToConstruction <= radius2) {
                        // Damage module
                        var mConditionIndicator = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", construction);
                        mConditionIndicator.decrease(1.0f);

                        Main.damageConstructions.Add(construction);
                        Main.ModuleExplodeParticle(construction);

                        // Register destroying components
                        var components = construction.getComponents();
                        foreach(var component in components)
                            if(Random.Range(0.0f, 1.0f) >= Main.settings.ProbabilityDropsExplosion)
                                destroyComponents.Add(component);
                    }
                }


                // Show message
                Texture2D icon = ContentManager.IconNuclearPlant;
                Singleton<MessageLog>.getInstance().addMessage(new Message("Nuclear Plant Explode", icon, module));

                // Destroy components
                foreach(var component in destroyComponents) {
                    ResourceAmounts resourceAmounts = component.calculateRecycleAmounts();
                    if(resourceAmounts != null) {
                        int num = 0;
                        foreach(ResourceAmount item in resourceAmounts) {
                            for(int i = 0; i < item.getAmount(); i++) {
                                if(Random.Range(0.0f, 1.0f) < Main.settings.ProbabilityDropsExplosion) {
                                    Vector3 vector = new Vector3((float)(num % 2) - 0.5f, 0f, (float)(num / 2 % 2) - 0.5f);
                                    Resource resource = Resource.create(item.getResourceType(), component.getPosition() + vector, Location.Interior);
                                    resource.setRotation(component.getTransform().rotation);
                                    resource.drop(Resource.State.Idle);
                                    num++;

                                    var mConditionIndicator = CoreUtils.GetMember<Resource, Indicator>("mConditionIndicator", resource);
                                    mConditionIndicator.decrease(Random.Range(0, 80) / 100.0f);
                                }
                            }
                        }
                    }

                    component.destroy();
                }

                Main.startDeletingConstructions = true;
            }
        }
    }
}
