using HarmonyLib;
using Planetbase;

namespace HeyHowAboutTheBasement.Patches
{
    [HarmonyPatch(typeof(Module), "linkModules")]
    public class linkModulesPatch
    {
        public static bool Prefix(Module m1, Module m2, bool renderTops)
        {
            if(m1.getModuleType() is ModuleTypeTunnelEntrance && m2.getModuleType() is ModuleTypeTunnelEntrance) {
                // Instantly build connection
                // TODO: connection between 2 tunnel entrances is inside mountain, no navigation route available. So build the connection immediately as temporary workaround
                Connection connection = Connection.create(m1, m2);
                connection.onUserPlaced();
                connection.onBuilt();
                connection.setRenderTop(renderTops);

                return false;
            }
            else {
                return true; // Vanilla mechanism
            }
        }
    }
}
