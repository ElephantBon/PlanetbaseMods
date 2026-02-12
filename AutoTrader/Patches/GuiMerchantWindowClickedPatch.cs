using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace AutoTrader.Patches
{
    [HarmonyPatch(typeof(GuiMerchantWindow))]
    internal class GuiMerchantWindowClickedPatch
    {
        [HarmonyPatch(nameof(GuiMerchantWindow.onMerchantStockClicked))]
        [HarmonyPrefix]
        public static bool onMerchantStockClicked(GuiMerchantWindow __instance, object parameter)
        {
            var mMerchantStockAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mMerchantStockAmounts", __instance);
            var mMerchantTradeAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mMerchantTradeAmounts", __instance);

            Product product = (Product)parameter;
            int num = GetClickAmount();
            for(int i = 0; i < num; i++) {
                if(mMerchantStockAmounts.getProductCount(product) <= 0) {
                    break;
                }

                mMerchantStockAmounts.remove(product, 1);
                mMerchantTradeAmounts.add(product, 1);
                CoreUtils.InvokeMethod("updateUi", __instance);
            }

            return false;
        }

        [HarmonyPatch(nameof(GuiMerchantWindow.onBaseStockClicked))]
        [HarmonyPrefix]
        public static bool onBaseStockClicked(GuiMerchantWindow __instance, object parameter)
        {
            var mBaseTradeAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mBaseTradeAmounts", __instance);
            var mBaseStockAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mBaseStockAmounts", __instance);
            var mMerchantShip = CoreUtils.GetMember<GuiMerchantWindow, MerchantShip>("mMerchantShip", __instance);

            Product product = (Product)parameter;
            int num = GetClickAmount();
            for(int i = 0; i < num; i++) {
                if(mBaseStockAmounts.getProductCount(product) <= 0) {
                    break;
                }

                if(!product.markForTrading(mMerchantShip)) {
                    break;
                }

                mBaseStockAmounts.remove(product, 1);
                mBaseTradeAmounts.add(product, 1);
            }

            CoreUtils.InvokeMethod("updateUi", __instance);
            return false;
        }

        [HarmonyPatch(nameof(GuiMerchantWindow.onMerchantTradeClicked))]
        [HarmonyPrefix]
        public static bool onMerchantTradeClicked(GuiMerchantWindow __instance, object parameter)
        {
            var mMerchantStockAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mMerchantStockAmounts", __instance);
            var mMerchantTradeAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mMerchantTradeAmounts", __instance);

            Product product = (Product)parameter;
            int num = GetClickAmount();
            for(int i = 0; i < num; i++) {
                if(mMerchantTradeAmounts.getProductCount(product) <= 0) {
                    break;
                }

                mMerchantStockAmounts.add(product, 1);
                mMerchantTradeAmounts.remove(product, 1);
                CoreUtils.InvokeMethod("updateUi", __instance);
            }
            return false;
        }

        [HarmonyPatch(nameof(GuiMerchantWindow.onBaseTradeClicked))]
        [HarmonyPrefix]
        public static bool onBaseTradeClicked(GuiMerchantWindow __instance, object parameter)
        {
            var mBaseTradeAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mBaseTradeAmounts", __instance);
            var mBaseStockAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mBaseStockAmounts", __instance);
            var mMerchantShip = CoreUtils.GetMember<GuiMerchantWindow, MerchantShip>("mMerchantShip", __instance);

            Product product = (Product)parameter;
            int num = GetClickAmount();
            for(int i = 0; i < num; i++) {
                if(mBaseTradeAmounts.getProductCount(product) <= 0) {
                    break;
                }

                mBaseStockAmounts.add(product, 1);
                mBaseTradeAmounts.remove(product, 1);
                product.unmarkForTrading(mMerchantShip.getId());
            }

            CoreUtils.InvokeMethod("updateUi", __instance);
            return false;
        }

        private static int GetClickAmount()
        {
            if(Event.current.shift)
                return 10;

            if(Event.current.control)
                return 100;

            return 1;
        }
    }
}
