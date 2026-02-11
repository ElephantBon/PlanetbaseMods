using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Reflection;
using TestMod.Objects;
using UnityEngine;
using UnityModManagerNet;
using KeyBinding = UnityModManagerNet.KeyBinding;

namespace TestMod
{
    static class Main
    {
        public static bool enabled;
        public static KeyBinding key = new KeyBinding() { keyCode = KeyCode.F1 };
        public static UnityModManager.ModEntry mod;


        static bool Load(UnityModManager.ModEntry modEntry)
        {
            // Patch with Harmony
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Reigister events
            mod = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;

            // Load assets
            ContentManager.Load();

            // Register string
            ModuleTypeTunnelEntrance.RegisterString();

            // Register new objects
            ModuleTypeList.getInstance().AddType(new ModuleTypeTunnelEntrance());

            return true;
        }

        // Called when the mod is turned to on/off.
        // With this function you control an operation of the mod and inform users whether it is enabled or not.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value /* to active or deactivate */)
        {
            if(value) {
                modEntry.Logger.Log("Mod active");
            }
            else {
                modEntry.Logger.Log("Mod deactivate"); 
            }

            enabled = value;
            return true; // If true, the mod will switch the state. If not, the state will not change.
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if(key.Down()) {
                modEntry.Logger.Log("F1 pressed");
            }
        }
    }
}
