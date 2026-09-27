using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Xml;

namespace HarshWorld.Patches
{
    [HarmonyPatch(typeof(DisasterManager))]
    internal class DisasterManagerPatch
    {
        [HarmonyPatch(nameof(DisasterManager.serialize))]
        [HarmonyPrefix]
        public static void serialize(DisasterManager __instance, XmlNode rootNode)
        {
            Singleton<CustomDisasterManager>.getInstance().serialize(rootNode);
        }

        [HarmonyPatch(nameof(DisasterManager.deserialize))]
        [HarmonyPrefix]
        public static void deserialize(DisasterManager __instance, XmlNode rootNode)
        {
            Singleton<CustomDisasterManager>.getInstance().deserialize(rootNode);
        }

        [HarmonyPatch(nameof(DisasterManager.update))]
        [HarmonyPrefix]
        public static void update(DisasterManager __instance, float timeStep)
        {
            Singleton<CustomDisasterManager>.getInstance().update(timeStep);
        }

        [HarmonyPatch(nameof(DisasterManager.anyInProgress))]
        [HarmonyPrefix]
        public static bool anyInProgress(DisasterManager __instance, ref bool __result)
        {
            if (Singleton<CustomDisasterManager>.getInstance().anyInProgress())
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(DisasterManager.onTimeScaleChanged))]
        [HarmonyPrefix]
        public static void onTimeScaleChanged(DisasterManager __instance, float timeScale, bool paused)
        {
            Singleton<CustomDisasterManager>.getInstance().onTimeScaleChanged(timeScale, paused);
        }
    }
}
