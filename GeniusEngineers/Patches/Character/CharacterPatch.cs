using HarmonyLib;
using Planetbase;
using System;
using UnityEngine;
using PlanetbaseModUtilities;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal class CharacterPatch
    {
        [HarmonyPatch(nameof(Character.create))]
        [HarmonyPatch(new Type[] { typeof(Specialization), typeof(Vector3), typeof(Location) })]
        [HarmonyPostfix]
        public static void create(Character __result, Specialization specialization, Vector3 position, Location location)
        {
            if(__result is Bot __instance) {
                var oldValue = CoreUtils.GetMember<Bot, float>("mIntegrityDecayRate", __instance);
                var newValue = oldValue * (100 + Main.ExtraBotLife) / 100;
                CoreUtils.SetMember("mIntegrityDecayRate", __instance, Math.Max(100, newValue));
            }
        }
    }
}
