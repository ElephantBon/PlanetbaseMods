using UnityEngine;
using UnityModManagerNet;

namespace HarshWorld
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Keybind to test disaster")] public KeyCode KeyTest = KeyCode.J;


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }

    }
}
