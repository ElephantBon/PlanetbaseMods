using UnityEngine;
using UnityModManagerNet;

namespace MoreHotkeys
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to toggle building enable/disable. Hold Shift to toggle all buildings of same type")] public KeyCode KeyToggleBuilding = KeyCode.G;
        [Draw("Keybind to edit component of selected building")] public KeyCode KeyEditComponent = KeyCode.E;
        [Draw("Keybind to recycle all components in selected building")] public KeyCode KeyRecycleAllComponents = KeyCode.P;
        [Draw("Keybind to complete selected construction or component instantly, or advance the progress of processor to 100%. Hold Shift to complete all or same type of the selected (Cheat)")] public KeyCode KeyInstantBuild = KeyCode.I;
        [Draw("Keybind to damage selected construction or component (Debug)")] public KeyCode KeyDamage = KeyCode.J;
        [Draw("Enable cheat")] public bool EnableCheat = true;
        [Draw("Enable debug")] public bool EnableDebug = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
