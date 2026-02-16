using HarmonyLib;
using Planetbase;
using UnityEngine;

namespace FreeBuilding
{
    [HarmonyPatch(typeof(Buildable), nameof(Buildable.setValidPosition))]
    public class setValidPositionPatch
    {
        // Colors from Bulidable class
        private static readonly Color ColorPlacingFreeMode = new Color(1f, 1f, 0f, 0.5f); // Yellow
        private static readonly Color ColorPlacingBeyondBorder = new Color(0.65f, 0.3f, 0.65f, 0.5f); // Purple

        public static void Postfix(Buildable __instance, bool validPosition)
        {
            if(validPosition && FreeBuilding.freeModeEnabled)
                if(FreeBuilding.placingBeyondBorder)
                    __instance.setColor(ColorPlacingBeyondBorder, immediate: true);
                else
                    __instance.setColor(ColorPlacingFreeMode, immediate: true);
        }
    }
}
