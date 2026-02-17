using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;
using WhereTheDeadBodies.Objects;
using static UnityModManagerNet.UnityModManager;

namespace WhereTheDeadBodies
{
    public class WhereTheDeadBodies : ModBase
    {
        // Variables
        private static bool corpseVisualApplied = false;

        public static new void Init(ModEntry modEntry)
        {
            InitializeMod(new WhereTheDeadBodies(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            ContentManager.Init(modEntry.Path);

            Corpse.RegisterString();
            Incinerator.RegisterString();
            ModuleTypeMorgue.RegisterString();

            CoreUtils.InvokeMethod("addResource", ResourceTypeList.getInstance(), new Corpse());
            ComponentTypeList.getInstance().AddType(new Incinerator());
            ModuleTypeList.getInstance().AddType(new ModuleTypeMorgue());
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Reset mod variables
            corpseVisualApplied = false;
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if(!corpseVisualApplied) {
                var resources = CoreUtils.GetMember<Resource, Dictionary<GameObject, Resource>>("mResourceDictionary").Values;
                if(resources != null && resources.Count > 0) {
                    corpseVisualApplied = true;
                    foreach(var resource in resources) {
                        if(resource.getResourceType() is Corpse)
                            Corpse.RandomVisual(resource);
                    }
                }
            }

            if(Input.GetKeyUp(KeyCode.C)) {
                var selected = Selection.getSelected();
                if(selected != null && selected is Character c) {
                    var r = Resource.create(ResourceTypeList.find<Corpse>(), c.getPosition() + MathUtil.randFlatVector(c.getRadius()), Location.Exterior);
                    r.drop(Resource.State.Idle);                    
                    c.destroy();
                }
            }
        }
    }
}
