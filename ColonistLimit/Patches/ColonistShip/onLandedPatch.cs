using HarmonyLib;
using Planetbase;
using UnityEngine;

namespace ColonistLimit.Patches
{
    [HarmonyPatch(typeof(ColonistShip), nameof(ColonistShip.onLanded))]
    internal class onLandedPatch
    {
        public static void Postfix(ColonistShip __instance)
        {
            int countOfType = Character.getCountOfType<Colonist>();
            int limit = Main.colonistLimit.get();
            if(limit > 0 && countOfType >= limit) {
                // Disable landing permission for colonists
                var landingPermissions = Singleton<LandingShipManager>.getInstance().getLandingPermissions();
                landingPermissions.getColonistRefBool().set(false);

                // Show message
                Texture2D icon = ResourceList.StaticIcons.LandingPermissions;
                Singleton<MessageLog>.getInstance().addMessage(new Message("Landing permission disabled on reaching colonist limit", icon, __instance));
            }
        }
    }
}
