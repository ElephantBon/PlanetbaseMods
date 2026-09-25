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
        [Draw("Probability of uranium ore produced in mine [max 1.00]")] public float ProbabilityUraniumOre = 0.01f;
        [Draw("Probability of resources survive in nuclear explosion [max 1.00]")] public float ProbabilityDropsExplosion = 0.3f;
        [Draw("Value of uranium ore")] public int ValueUraniumOre = 120;
        [Draw("Value of uranium rod")] public int ValueUraniumRod = 100;


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }

    }
}
