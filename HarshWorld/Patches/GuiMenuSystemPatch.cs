using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace HarshWorld.Patches
{
    [HarmonyPatch(typeof(GuiMenuSystem))]
    internal class GuiMenuSystemPatch
    {
        [HarmonyPatch(nameof(GuiMenuSystem.setActionMenu))]
        [HarmonyPostfix]
        public static void setActionMenu(GuiMenuSystem __instance)
        {
            var construction = Selection.getSelectedConstruction();
            if (construction == null)
            {
                return;
            }

            if (construction.isLockable(out bool buttonLock))
            {
                var menuAction = CoreUtils.GetMember<GuiMenuSystem, GuiMenu>("mMenuAction", __instance);

                var lockItem = new GuiMenuItem(
                    buttonLock ? AssetManager.IconLock : AssetManager.IconUnlock,
                    buttonLock ? "Lock to block passage" : "Unlock to allow passage",
                    onButtonLock);
                Singleton<ShortcutManager>.getInstance().addShortcut(HarshWorld.settings.KeyToggleLock, lockItem.onShortcut);

                Indicator condition = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", construction);
                lockItem.setEnabled(condition == null || condition.getLevel() > IndicatorLevel.ExtremelyLow);

                menuAction.addItem(lockItem);
            }
        }

        public static void onOpenDisasterMenu()
        {
            var gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            if (gameStateGame == null)
            {
                return;
            }

            var gameGui = CoreUtils.GetMember<GameStateGame, GameGui>("mGameGui", gameStateGame);
            if (gameGui.getWindow() is GuiDisasterGameMenu)
            {
                gameGui.setWindow(null);
                return;
            }

            gameGui.setWindow(new GuiDisasterGameMenu());
        }

        public static void onButtonLock(object parameter)
        {
            var construction = Selection.getSelectedConstruction();
            if (construction == null)
            {
                return;
            }

            bool isLocked = !construction.isLocked();
            construction.setLocked(isLocked);
            ConstructionPatch.SetUserLocked(construction, isLocked);
        }

        private static string getDisasterLabel(Disaster disaster)
        {
            if (disaster is CropDisease)
            {
                return "Crop Disease";
            }

            if (disaster is FireHazard)
            {
                return "Fire Hazard";
            }

            if (disaster is MeteorRain)
            {
                return "Meteor Rain";
            }

            return disaster.GetType().Name;
        }

        private static void closeDisasterMenu(object parameter)
        {
            var gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            if (gameStateGame == null)
            {
                return;
            }

            var gameGui = CoreUtils.GetMember<GameStateGame, GameGui>("mGameGui", gameStateGame);
            gameGui.setWindow(null);
        }

        private class GuiDisasterGameMenu : GuiGameMenu
        {
            public GuiDisasterGameMenu()
            {
                var disasters = Singleton<CustomDisasterManager>.getInstance().getDisasters();
                for (int i = 0; i < disasters.Length; i++)
                {
                    var disaster = disasters[i];
                    var button = new GuiButtonItem(getDisasterLabel(disaster), _ => disaster.trigger(), FontType.Title);
                    mRootItem.addChild(button);
                }

                var returnButton = new GuiButtonItem("Return to game", closeDisasterMenu, FontType.Title);
                mRootItem.addChild(returnButton);
            }
        }
    }
}
