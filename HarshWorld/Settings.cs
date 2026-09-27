using UnityEngine;
using UnityModManagerNet;

namespace HarshWorld
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to test disaster")] public KeyCode KeyTest = KeyCode.J;
        [Draw("Minimum duration between meteor rains (seconds)")] public float MinimumDurationBetweenMeteorRains = 2400f;
        [Draw("Maximum duration between meteor rains (seconds)")] public float MaximumDurationBetweenMeteorRains = 3600f;
        [Draw("Duration of meteor rains (seconds)")] public float MeteorRainDuration = 15f;


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }

    }
}
