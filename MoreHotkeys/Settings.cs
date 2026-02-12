using UnityEngine;
using UnityModManagerNet;

namespace MoreHotkeys
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind for toggle building enable/disable")] public KeyCode KeyToggleBuilding = KeyCode.G;
        [Draw("Keybind for recycle all components in selected building")] public KeyCode KeyRecycleAllComponents = KeyCode.P;
        [Draw("Keybind for build selected construction instantly")] public KeyCode KeyInstantBuild = KeyCode.I;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
