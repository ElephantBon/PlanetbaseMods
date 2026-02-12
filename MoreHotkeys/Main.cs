using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;


namespace MoreHotkeys
{
    public class Main : ModBase
    {
        public static Settings settings;

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
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            #region Construction
            {
                var construction = Selection.getSelectedConstruction();
                if(construction != null) {
                    // Recycle all components
                    if(Input.GetKeyUp(settings.KeyRecycleAllComponents)) {
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
                        if(construction.canBeDisabled()) {
                            construction.setEnabled(!construction.isEnabled());

                            if(construction.isEnabled())
                                Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ConstructionEnable, construction);
                            else
                                Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ConstructionDisable, construction);
                        }
                    }
                    else
                    if(Input.GetKeyUp(settings.KeyInstantBuild)) {
                        if(!construction.isBuilt())
                            construction.onBuilt();
                    }
                    return;
                }
            }
            #endregion

            #region Component
            {
                var component = Selection.getSelectedBuildable() as ConstructionComponent;
                if(component != null) {
                    if(Input.GetKeyUp(settings.KeyInstantBuild)) {
                        if(!component.isBuilt())
                            component.onBuilt();
                    }
                    return;
                }
            }
            #endregion
        }
    }
}
