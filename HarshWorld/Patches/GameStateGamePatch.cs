using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshWorld.Patches
{
    internal class GameStateGamePatch
    {

        [HarmonyPatch(nameof(GameStateGame.destroy))]
        [HarmonyPrefix]
        public static void destroy(GameStateGame __instance)
        {
            Singleton<CustomDisasterManager>.getInstance().destroy();
        }
    }
}
