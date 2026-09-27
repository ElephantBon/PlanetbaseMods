using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HarshWorld.Patches
{
    /// <summary>
    /// Construction only stays locked on high condition when the player locked it manually.
    /// </summary>
    [HarmonyPatch(typeof(Construction))]
    internal class ConstructionPatch
    {
        private static readonly HashSet<Construction> mUserLockedConstructions = new HashSet<Construction>();

        public static bool IsUserLocked(Construction construction)
        {
            return construction != null && mUserLockedConstructions.Contains(construction);
        }

        public static void SetUserLocked(Construction construction, bool isLocked)
        {
            if (construction == null)
            {
                return;
            }

            if (isLocked)
            {
                mUserLockedConstructions.Add(construction);
            }
            else
            {
                mUserLockedConstructions.Remove(construction);
            }
        }

        public static void ClearUserLocks()
        {
            mUserLockedConstructions.Clear();
        }

        [HarmonyPatch(nameof(Construction.tick))]
        [HarmonyPrefix]
        public static void TickPrefix(Construction __instance, ref bool __state)
        {
            __state = Traverse.Create(__instance).Field("mLocked").GetValue<bool>();
        }

        [HarmonyPatch(nameof(Construction.tick))]
        [HarmonyPostfix]
        public static void TickPostfix(Construction __instance, bool __state)
        {
            if (!__state || !IsUserLocked(__instance))
            {
                return;
            }

            Traverse construction = Traverse.Create(__instance);
            bool isLocked = construction.Field("mLocked").GetValue<bool>();
            if (!isLocked)
            {
                construction.Method("setLocked", new object[] { true }).GetValue();
            }
        }
    }
}
