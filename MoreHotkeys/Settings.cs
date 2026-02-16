using UnityEngine;
using UnityModManagerNet;

namespace MoreHotkeys
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to toggle building enable/disable, hold Shift to toggle all buildings of same type")] public KeyCode KeyToggleBuilding = KeyCode.G;
        [Draw("Keybind to edit component of selected building")] public KeyCode KeyEditComponent = KeyCode.H;
        [Draw("Keybind for recycle all components in selected building")] public KeyCode KeyRecycleAllComponents = KeyCode.P;
        [Draw("Keybind for build selected construction instantly (Cheat)")] public KeyCode KeyInstantBuild = KeyCode.I;
        [Draw("Enable cheat")] public bool EnableCheat = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
