using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Xml;

namespace ColonistLimit.Patches
{
    [HarmonyPatch(typeof(LandingPermissions), nameof(LandingPermissions.serialize))]
    internal class serializePatch
    {
        public static bool Prefix(LandingPermissions __instance, XmlNode parent, string name)
        {
            var mColonistsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mColonistsAllowed", __instance);
            var mVisitorsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mVisitorsAllowed", __instance);
            var mMerchantsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mMerchantsAllowed", __instance);
            var mSpecializationPercentages = CoreUtils.GetMember<LandingPermissions, Dictionary<Specialization, RefInt>>("mSpecializationPercentages", __instance);

            XmlNode parent2 = Serialization.createNode(parent, name);
            Serialization.serializeBool(parent2, "colonists-allowed", mColonistsAllowed.get());
            Serialization.serializeBool(parent2, "merchants-allowed", mMerchantsAllowed.get());
            Serialization.serializeBool(parent2, "visitors-allowed", mVisitorsAllowed.get());
            foreach(Specialization key in mSpecializationPercentages.Keys) {
                string name2 = key.GetType().Name;
                Serialization.serializeInt(parent2, name2 + "-percentage", mSpecializationPercentages[key].get());
            }
            Serialization.serializeInt(parent2, "intColonistLimit", Main.colonistLimit.get());
            return false;
        }
    }

    
    [HarmonyPatch(typeof(LandingPermissions), nameof(LandingPermissions.deserialize))]
    internal class deserializePatch
    { 
        public static bool Prefix(LandingPermissions __instance, XmlNode node)
        {
            if(node == null) {
                return false;
            }

            var mColonistsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mColonistsAllowed", __instance);
            var mVisitorsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mVisitorsAllowed", __instance);
            var mMerchantsAllowed = CoreUtils.GetMember<LandingPermissions, RefBool>("mMerchantsAllowed", __instance);
            var mSpecializationPercentages = CoreUtils.GetMember<LandingPermissions, Dictionary<Specialization, RefInt>>("mSpecializationPercentages", __instance);

            mColonistsAllowed.set(Serialization.deserializeBool(node["colonists-allowed"]));
            mMerchantsAllowed.set(Serialization.deserializeBool(node["merchants-allowed"]));
            mVisitorsAllowed.set(Serialization.deserializeBool(node["visitors-allowed"]));
            foreach(Specialization colonistSpecialization in SpecializationList.getColonistSpecializations()) {
                string name = colonistSpecialization.GetType().Name;
                mSpecializationPercentages[colonistSpecialization].set(Serialization.deserializeInt(node[name + "-percentage"]));
            }
            Main.colonistLimit.set(Serialization.deserializeInt(node["intColonistLimit"]));
            return false;
        }
    }
}
