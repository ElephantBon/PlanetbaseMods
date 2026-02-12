using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(TechManager), nameof(TechManager.acquire))]
    internal class acquirePatch
    {
        public static void Postfix(Tech tech)
        {
            if(tech is TechNuclearPlant)
                Main.UpdateGuiNuclearPlant();
        }
    }
}
