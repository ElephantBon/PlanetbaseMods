using AssetUtility;
using HarmonyLib;
using NuclearPlant.Objects;
using Planetbase;
using System.IO;
using UnityEngine;

namespace NuclearPlant.Patches
{
    /// <summary>
    /// Add Nuclear Core prefab to center of Nuclear Plant module
    /// </summary>
    [HarmonyPatch(typeof(Construction), "onBuilt")]
    internal class onBuiltPatch
    {
        public static void Postfix(Construction __instance)
        {
            if(!(__instance is Module module && module.getModuleType() is ModuleTypeNuclearPlant))
                return;

            var model = AssetUtils.LoadGameObject(Path.Combine(Main.ModEntry.Path, "Assets\\nuclearplant.assetbundle"), "NuclearCore");

            GameObject obj = Object.Instantiate<GameObject>(model);
            obj.SetActive(true);

            var mObject = __instance.getGameObject();
            obj.transform.SetParent(mObject.transform, false);
            obj.transform.localScale = new Vector3(120.0f, 120.0f, 120.0f);
            obj.layer = 11;
            obj.disablePhysics();
        }
    }
}
