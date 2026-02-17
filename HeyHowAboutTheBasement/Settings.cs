using UnityModManagerNet;

namespace HeyHowAboutTheBasement
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Moving walkway speed multiplier")] public float MovingWalkwaySpeedMultiplier = 4.0f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
