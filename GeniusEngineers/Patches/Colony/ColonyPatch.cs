using HarmonyLib;
using Planetbase;
using System.Xml;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(Colony))]
    internal class ColonyPatch
    {
        [HarmonyPatch(nameof(Colony.serialize))]
        [HarmonyPostfix]
        public static void serialize(Colony __instance, XmlNode rootNode, string name)
        {
            XmlNode node = rootNode[name];
            Serialization.serializeFloat(node, nameof(Main.ExtraBotLife), Main.ExtraBotLife);
            Serialization.serializeFloat(node, nameof(Main.ExtraVegetableLife), Main.ExtraVegetableLife);
            Serialization.serializeFloat(node, nameof(Main.ExtraPowerStorage), Main.ExtraPowerStorage);
            Serialization.serializeFloat(node, nameof(Main.ExtraWaterStorage), Main.ExtraWaterStorage);
        }

        [HarmonyPatch(nameof(Colony.deserialize))]
        [HarmonyPostfix]
        public static void deserialize(Colony __instance, XmlNode node)
        {
            if(node != null) {
                Main.ExtraBotLife = Serialization.deserializeFloat(node[nameof(Main.ExtraBotLife)]);
                Main.ExtraVegetableLife = Serialization.deserializeFloat(node[nameof(Main.ExtraVegetableLife)]);
                Main.ExtraPowerStorage = Serialization.deserializeFloat(node[nameof(Main.ExtraPowerStorage)]);
                Main.ExtraWaterStorage = Serialization.deserializeFloat(node[nameof(Main.ExtraWaterStorage)]);
            }
        }
    }
}
