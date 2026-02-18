using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using WhereTheDeadBodies.Objects;
using static UnityModManagerNet.UnityModManager;

namespace WhereTheDeadBodies
{
    public class WhereTheDeadBodies : ModBase
    {
        public static Settings settings;

        private const float infectionTimerInterval = 60f;
        private static float infectionTimerCounter = 0;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            InitializeMod(new WhereTheDeadBodies(), modEntry);
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

            Corpse.RegisterString();
            Remains.RegisterString();
            Incinerator.RegisterString();
            ModuleTypeMorgue.RegisterString();

            ResourceTypeList.getInstance().AddType(new Corpse());
            ResourceTypeList.getInstance().AddType(new Remains());
            ComponentTypeList.getInstance().AddType(new Incinerator());
            ModuleTypeList.getInstance().AddType(new ModuleTypeMorgue());
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            InfectionOutbreak(timeStep);

            if(settings.EnableDebug && Input.GetKeyUp(KeyCode.C)) {
                infectionTimerCounter = infectionTimerInterval;
            }
        }

        private void InfectionOutbreak(float timeStep)
        {
            infectionTimerCounter += timeStep;
            if(infectionTimerCounter < infectionTimerInterval)
                return;
            else
                infectionTimerCounter = 0;

            if(CoreUtils.GetMember<Character, List<Character>>("mCharacters").Any(x => x.getCondition()?.getConditionType() is ConditionFlu)) {
                if(settings.EnableDebug)
                    Singleton<MessageLog>.getInstance().addMessage(new Message($"Found existing infected, skip the event."));
                return; // Skip new infection if there are already people infected, let the built-in propagation mechnism do more infection
            }

            var fluCounter = 0;
            var flu = ConditionTypeList.find<ConditionFlu>();
            var corpses = CoreUtils.GetMember<Resource, List<Resource>>("mResources").Where(x => x.getResourceType() is Corpse && x.getLocation() == Location.Interior && !x.isEmbedded() && x.getState() == Resource.State.Idle).ToList();
            var healthyHumans = CoreUtils.GetMember<Character, List<Character>>("mCharacters").Where(x => x is Human human && (human.getCondition() == null || !(human.getCondition().getConditionType() is ConditionFlu))).ToList();
            var probability = settings.ProbabilityInfection * corpses.Count;
            if(probability > 0) {
                foreach(var human in healthyHumans) {
                    if(Random.Range(0.0f, 1.0f) < probability) {
                        human.setCondition(flu);
                        fluCounter++;
                    }
                }
            }

            if(settings.EnableDebug)
                Singleton<MessageLog>.getInstance().addMessage(new Message($"corpses={corpses.Count}, heathyHumans={healthyHumans.Count}, infected={fluCounter}, probability={probability}"));
            else
            if(fluCounter > 0)
                Singleton<MessageLog>.getInstance().addMessage(new Message($"An infection outbreak is occurring. {fluCounter} people are infected."));
            

        }
    }
}
