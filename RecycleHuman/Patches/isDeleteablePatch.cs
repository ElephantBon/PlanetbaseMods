using HarmonyLib;
using Planetbase;

namespace RecycleHuman.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.isDeleteable))]
    public class isDeleteablePatch
    {
        public static bool Prefix(ref bool __result, Character __instance, out bool buttonEnabled)
        {
            // To show recycle button for human
            buttonEnabled = true;
            if(__instance is Human) {
                __result = true;
                return false; // Disable original code
            }
            else
                return true;
        }
    }
}
