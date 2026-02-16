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
                            if(m.getModuleType().Equals(module.getModuleType()) && m.canBeDisabled())
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
                var gameState = GameStateUtils.GetGameStateGame();
                if(gameState != null) { 
                    var selected = Selection.getSelected();
                    if(selected.isEditable())
                        gameState.onButtonEdit(selected);
                }
            }
            else
            if(settings.EnableCheat && Input.GetKeyUp(settings.KeyInstantBuild)) {                
                if(Input.GetKey(KeyCode.LeftShift)) {
                    // Build all constructions, modules and components with or without selection
                    var selected = Selection.getSelected();
                    BuildableUtils.GetAllConstructions().ForEach(m => {
                        if(!m.isBuilt() && (selected == null || (selected is Module m1 && m is Module m2 && m1.getModuleType().Equals(m2.getModuleType())) || m is Connection)) 
                            m.onBuilt();
                    });
                    BuildableUtils.GetAllComponents().ForEach(c => {
                        if(!c.isBuilt() && (selected == null || (selected is ConstructionComponent component && component.getComponentType().Equals(c.getComponentType()))))
                            c.onBuilt();
                    });
                }
                else {
                    // Build selected construction, module or component
                    var construction = Selection.getSelectedConstruction();
                    if(construction != null) {
                        if(!construction.isBuilt()) 
                            construction.onBuilt();
                        return;
                    }                    

                    var component = Selection.getSelectedBuildable() as ConstructionComponent;
                    if(component != null) {
                        if(!component.isBuilt())
                            component.onBuilt();
                        return;
                    }
                }
            }
        }
    }
}
