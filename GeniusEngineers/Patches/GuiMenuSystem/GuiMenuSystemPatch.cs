using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(GuiMenuSystem))]
    internal class GuiMenuSystemPatch
    {
        [HarmonyPatch(nameof(GuiMenuSystem.init))]
        [HarmonyPostfix]
        public static void init(GuiMenuSystem __instance, GameStateGame gameStateGame)
        {
            var mMenuBaseManagement = MenuUtils.GetMenu(__instance, "mMenuBaseManagement");
            var menuItem = new GuiMenuItem(ResourceList.StaticIcons.Bot, "Research", gameStateGame.toggleWindow<GuiResearchesWindow>);
            MenuUtils.AddItemBeforeBackItem(mMenuBaseManagement, menuItem);
        }
    }
}
