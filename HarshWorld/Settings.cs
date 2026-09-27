using UnityEngine;
using UnityModManagerNet;

namespace HarshWorld
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to test disaster")] public KeyCode KeyTest = KeyCode.J;

        // Crop Disease Settings
        [Draw("Crop Disease: Minimum duration between disaster (seconds)")] public float MinimumDurationBetweenCropDiseases = 2400f;
        [Draw("Crop Disease: Maximum duration between disaster (seconds)")] public float MaximumDurationBetweenCropDiseases = 3600f;
        [Draw("Crop Disease: Duration of disaster (seconds)")] public float CropDiseaseDuration = 120f;
        [Draw("Crop Disease: Spread Interval (seconds)")] public float CropDiseaseSpreadInterval = 10f;
        [Draw("Crop Disease: Maximum percentage of plants affected by disaster in every cycle (%)")] public float CropDiseaseSpreadPercentage = 20f;

        // Meteor Rain Settings
        [Draw("Meteor Rain: Minimum duration between disaster (seconds)")] public float MinimumDurationBetweenMeteorRains = 2400f;
        [Draw("Meteor Rain: Maximum duration between disaster (seconds)")] public float MaximumDurationBetweenMeteorRains = 3600f;
        [Draw("Meteor Rain: Duration of disaster (seconds)")] public float MeteorRainDuration = 15f;


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }

    }
}
