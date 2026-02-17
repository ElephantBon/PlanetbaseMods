using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;


namespace MoreHotkeys
{
    public class MoreHotkeys : ModBase
    {
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new MoreHotkeys(), modEntry);
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
            if(Input.GetKeyUp(settings.KeyRecycleAllComponents)) {
                var construction = Selection.getSelectedConstruction();
                if(construction == null)
                    return; // No selection

                var components = CoreUtils.GetMember<Construction, List<ConstructionComponent>>("mComponents", construction);

                bool playSound = false;
                while(components.Count > 0) {
                    playSound = true;
                    var component = components.ElementAt(0);
                    component.recycle();
                    component.destroy();
                }

                if(playSound)
                    Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ConstructionRecycle, construction);
            }
            else
            if(Input.GetKeyUp(settings.KeyToggleBuilding)) {
                var construction = Selection.getSelectedConstruction();
                if(construction == null)
                    return; // No selection

                if(construction is Module module) { 
                    var isEnabled = module.isEnabled();

                    if(Input.GetKey(KeyCode.LeftShift)) 
                        BuildableUtils.GetAllModules().ForEach(m => {
                            if(m.getModuleType() == module.getModuleType() && m.canBeDisabled())
                                m.setEnabled(!isEnabled);
                            });
                    else
                        if(module.canBeDisabled()) 
                            module.setEnabled(!isEnabled);

                    if(!isEnabled)
                        Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ConstructionEnable, module);
                    else
                        Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ConstructionDisable, module);
                }
            }
            else
            if(Input.GetKeyUp(settings.KeyEditComponent)) {
                var gameState = GameStateUtils.GetGameStateGameUpdating();
                if(gameState != null) { 
                    var selected = Selection.getSelected();
                    if(selected.isEditable())
                        gameState.onButtonEdit(selected);
                }
            }
            else
            if(settings.EnableCheat && Input.GetKeyUp(settings.KeyInstantBuild)) {                
                var selected = Selection.getSelectedBuildable();
                if(selected == null) {
                    if(Input.GetKey(KeyCode.LeftShift)) { 
                        // Build all constructions, modules and components
                        BuildableUtils.GetAllConstructions().ForEach(c => {
                            if(!c.isBuilt())
                                c.onBuilt();
                        });
                        BuildableUtils.GetAllComponents().ForEach(c => {
                            if(!c.isBuilt())
                                c.onBuilt();
                        });
                    }
                }
                else {
                    if(Input.GetKey(KeyCode.LeftShift)) {
                        // Build or finish progress of all same type of the selected
                        if(selected is Module m1)
                            BuildableUtils.GetAllConstructions().ForEach(c => {
                                if(c is Module m2 && m1.getModuleType() == m2.getModuleType()) { 
                                    if(!c.isBuilt())
                                        c.onBuilt();
                                    var indicator = c.getIndicators().FirstOrDefault(i => i.getType() == IndicatorType.Progress);
                                    if(indicator != null)
                                        indicator.increase(1000000);                                                    
                                }
                            });
                        else
                        if(selected is ConstructionComponent c1) 
                            BuildableUtils.GetAllComponents().ForEach(c => {
                                if(c is ConstructionComponent c2 && c1.getComponentType() == c2.getComponentType()) { 
                                    if(!c.isBuilt())
                                        c.onBuilt();
                                    var indicator = c.getIndicators().FirstOrDefault(i => i.getType() == IndicatorType.Progress);
                                    if(indicator != null)
                                        indicator.increase(1000000);                                                    
                                }
                            });
                    }
                    else {
                        // Build or finish progress of the selected
                        if(!selected.isBuilt())
                            selected.onBuilt();
                        var indicator = selected.getIndicators().FirstOrDefault(i => i.getType() == IndicatorType.Progress);
                        if(indicator != null)
                            indicator.increase(1000000);
                    }
                }
            }
            else
            if(settings.EnableDebug && Input.GetKeyUp(settings.KeyDamage)) {
                var selected = Selection.getSelected();
                if(selected == null) {
                    // Destroy all resources on ground
                    CoreUtils.GetMember<Resource, List<Resource>>("mResources").ToList().ForEach(r => {
                        if(r.isDeleteable(out bool deletable) && deletable) { 
                            var indicator = r.getIndicators().FirstOrDefault(i => i.getType() == IndicatorType.Condition);
                            if(indicator != null && indicator.getValue() < 100)
                                r.destroy();
                        }
                    });
                } 
                else {
                    // Damage selected bot or human
                    if(selected is Character c) {
                        c.decayIndicator(CharacterIndicator.Health, 1f);
                        c.decayIndicator(CharacterIndicator.Condition, 1f);
                    }
                    else { 
                        // Damage selected construction or component
                        var indicator = selected.getIndicators().FirstOrDefault(i => i.getType() == IndicatorType.Condition);
                        if(indicator != null)
                            indicator.decrease(1000000);
                    }
                }
            }
        }
    }
}