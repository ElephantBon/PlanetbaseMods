using UnityEngine;
using UnityModManagerNet;

namespace FreeBuilding
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to toggle free building mode")] public KeyCode KeyToggleFreeMode = KeyCode.F;
        [Draw("Keybind to switch facing if multiple links available")] public KeyCode KeyRotation = KeyCode.R;
        [Draw("Complete connections instantly to prevent engineer stuck")] public bool InstantConnection = false;
        [Draw("Allow build beyond border, but people are not able to navigate there and the placement turns purple as a warning")] public bool AllowBeyondBorder = false;
        [Draw("Minimum distance")] public float MinimumDistance = 3f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
