using UnityEngine;
using UnityModManagerNet;

namespace HarshWorld
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to open disaster menu for testing")] public KeyCode KeyDisasterMenu = KeyCode.H;

        // Crop Disease Settings
        [Draw("Crop Disease: Minimum duration between disaster (seconds)")] public float MinimumDurationBetweenCropDiseases = 2400f;
        [Draw("Crop Disease: Maximum duration between disaster (seconds)")] public float MaximumDurationBetweenCropDiseases = 3600f;
        [Draw("Crop Disease: Duration of disaster (seconds)")] public float CropDiseaseDuration = 120f;
        [Draw("Crop Disease: Spread interval (seconds)")] public float CropDiseaseSpreadInterval = 10f;
        [Draw("Crop Disease: Maximum percentage of plants affected by disaster in every cycle (%)")] public float CropDiseaseSpreadPercentage = 20f;

        // Fire Hazard Settings
        [Draw("Fire Hazard: Minimum duration between disaster (seconds)")] public float MinimumDurationBetweenFireHazards = 2400f;
        [Draw("Fire Hazard: Maximum duration between disaster (seconds)")] public float MaximumDurationBetweenFireHazards = 3600f;
        [Draw("Fire Hazard: Duration of disaster (seconds)")] public float FireHazardDuration = 3600f;
        [Draw("Fire Hazard: Spread probability (%)")] public float FireSpreadProbability = 2f;
        [Draw("Fire Hazard: Component and resource burn down probability (%)")] public float FireBurnDownProbability = 1f;
        [Draw("Fire Hazard: Damage strength")] public float FireDamageStrength = 0.1f;
        [Draw("Keybind to lock/unlock selected construction")] public KeyCode KeyToggleLock = KeyCode.L;

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
