using UnityModManagerNet;

namespace WhereTheDeadBodies
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Probability of infection outbreak")] public float ProbabilityInfection = 0.01f;
        [Draw("Enable debugging")] public bool EnableDebug = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
