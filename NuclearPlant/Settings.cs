using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace NuclearPlant
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Uranium ore requires technology")] public bool uraniumOreRequiresTech = false;
        [Draw("Enable hotkey")] public bool enableDebugHotkey = true;
        [Draw("Keybind to worship Atom")] public KeyCode KeyWorshipAtom = KeyCode.N; // Acquire the technology if not acquired. Detonate Nuclear Plant if technology is already acquired

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
