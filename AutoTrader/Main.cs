using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Reflection;
using UnityEngine.UI;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.Param;


namespace AutoTrader
{
    public class Main : ModBase
    {
        public static new void Init(ModEntry modEntry)
        {
            InitializeMod(new Main(), modEntry);
        }

        public override void OnInitialized(ModEntry modEntry)
        {
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
        }
    }
}
