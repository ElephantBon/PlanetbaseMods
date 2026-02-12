using UnityModManagerNet;

namespace HeyHowAboutTheBasement
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("No time to explain")] public bool NoTimeToExplain = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
        void IDrawable.OnChange()
        {
        }
    }
}
