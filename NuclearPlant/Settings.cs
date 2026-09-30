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
        [Draw("Uranium ore requires technology")] public bool uraniumOreRequiresTech = true;
        [Draw("Enable hotkey")] public bool enableDebugHotkey = false;
        [Draw("Keybind to worship Atom")] public KeyCode KeyWorshipAtom = KeyCode.N; // Acquire the technology if not acquired. Detonate Nuclear Plant if technology is already acquired
        [Draw("Probability of uranium ore produced in mine (%)")] public float ProbabilityUraniumOrePercentage = 1f;
        [Draw("Probability of resources survive in nuclear explosion (%)")] public float ProbabilityDropsExplosionPercentage = 30f;
        [Draw("Value of uranium ore, need to restart game")] public int ValueUraniumOre = 120;
        [Draw("Value of uranium rod, need to restart game")] public int ValueUraniumRod = 100;
        [Draw("Nuclear plant condition decay time (s), need to restart game")] public float NuclearPlantConditionDecayTime = 24000;
        [Draw("Nuclear plant maintain factor")] public float NuclearPlantMaintainFactor = 2.0f;
        [Draw("Nuclear plant degrade factor")] public float NuclearPlantDegradeFactor = 1.0f;
        [Draw("Nuclear plant insufficient degrade factor")] public float NuclearPlantInsufficientDegradeFactor = 5.0f;


        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }

    }
}
