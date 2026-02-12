using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;

namespace RecycleHuman.Patches
{
    [HarmonyPatch(typeof(GuiMenuSystem), nameof(GuiMenuSystem.setActionMenu))]
    internal class setActionMenuPatch
    {
        public static void Postfix(GuiMenuSystem __instance)
        {
            var selected = Selection.getSelected();
            if(selected == null || !(selected is Human))
                return;

            // Replace callback of recycle button for human
            var mItemRecycle = CoreUtils.GetMember<GuiMenuSystem, GuiMenuItem>("mItemRecycle", __instance);
            if(mItemRecycle != null) 
                mItemRecycle.SetCallback(showGuiRecycleConfirm);
        }

        private static void showGuiRecycleConfirm(object parameter)
        {
            // Replace recycle resources
            var gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            var mGameGui = CoreUtils.GetMember<GameStateGame, GameGui>("mGameGui", gameStateGame);
            mGameGui.setWindow(new GuiConfirmWindow(StringList.get("confirm_recycle"), onRecycle, null, getHumanResources(), 1));
        }

        private static void onRecycle(object parameter)
        {
            // Create resources
            var selected = Selection.getSelected();
            foreach(ResourceAmount item in getHumanResources()) {
                for(int i = 0; i < item.getAmount(); i++) {
                    Resource resource = Resource.create(item.getResourceType(), selected.getPosition() + MathUtil.randFlatVector(selected.getRadius()), selected.getLocation());
                    resource.drop(Resource.State.Idle);
                }
            }

            // Run original code
            var gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            gameStateGame.onRecycle(null);

            // Decrease morale of all colonists
            var colonists = CoreUtils.GetMember<Character, List<Character>>("mCharacters").Where(x => x is Colonist).ToArray();
            foreach(var colonist in colonists) {
                var currentMorale = colonist.getIndicators().ToArray()[(int)CharacterIndicator.Morale].getValue();
                var decayAmount = currentMorale * Main.moraleDecayPercentage;
                colonist.decayIndicator(CharacterIndicator.Morale, decayAmount);
            }
        }

        private static ResourceAmounts getHumanResources()
        {
            var resourceAmounts = new ResourceAmounts();
            resourceAmounts.add(ResourceTypeList.VitromeatInstance, 3);
            return resourceAmounts;
        }
    }
}
