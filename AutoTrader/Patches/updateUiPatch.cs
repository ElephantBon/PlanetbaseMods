using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;

namespace AutoTrader.Patches
{
    [HarmonyPatch(typeof(GuiMerchantWindow), "updateUi")]
    internal class updateUiPatch
    {
        public static void Postfix(GuiMerchantWindow __instance)
        {
            var mRootItem = __instance.getRootItem();

            GuiLabelItem guiLabelItem3 = new GuiLabelItem("Auto Coins", Util.applyColor(ResourceUtil.loadIcon("Resources/icon_coins")));
            guiLabelItem3.addCallback(onAutoCoins, __instance);
            var guiRowItem2 = mRootItem.getLastChild();
            guiRowItem2.addChild(guiLabelItem3);
            guiLabelItem3.setEnabled(true);
        }

        public static void onAutoCoins(object parameter)
        {
            var __instance = (GuiMerchantWindow) parameter;
            var mMerchantShip = CoreUtils.GetMember<GuiMerchantWindow, MerchantShip>("mMerchantShip", __instance);
            var mMerchantTradeAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mMerchantTradeAmounts", __instance);
            var mBaseTradeAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mBaseTradeAmounts", __instance);

            int baseValue = mBaseTradeAmounts.getValue();
            int commission = mMerchantShip.getCommission();
            int merchantValue = (100 + commission) * mMerchantTradeAmounts.getValue() / 100;

            int coinsRequired = merchantValue - baseValue;
            if(coinsRequired > 0) {
                // Move coins from base to merchant
                var mBaseStockAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mBaseStockAmounts", __instance);
                foreach(var amount in mBaseStockAmounts) {
                    var product = amount.getProduct();
                    if(product is ProductResource resource && resource.getResourceType() is Coins) {
                        var coinsToTrade = Math.Min(coinsRequired, amount.getAmount());
                        for(int i = 0; i < coinsToTrade; i++) {
                            mBaseStockAmounts.remove(product, 1);
                            mBaseTradeAmounts.add(product, 1);

                            if(!product.markForTrading(mMerchantShip)) 
                                break;                            
                        }
                        CoreUtils.InvokeMethod("updateUi", __instance);
                        return;
                    }
                }
            }
            else
            if(coinsRequired < 0) {
                // Move coins from merchant to base
                coinsRequired = Math.Abs(coinsRequired);
                int coinsMerchant = (int)Math.Ceiling((float)(coinsRequired * 100 / (float)(100 + commission)));
                while(true) {
                    int valueMerchant = (100 + commission) * coinsMerchant / 100;
                    if(valueMerchant > coinsRequired)
                        coinsMerchant--;
                    else
                        break;
                }

                var mMerchantStockAmounts = CoreUtils.GetMember<GuiMerchantWindow, ProductAmounts>("mMerchantStockAmounts", __instance);
                foreach(var amount in mMerchantStockAmounts) {
                    var product = amount.getProduct();
                    if(product is ProductResource resource && resource.getResourceType() is Coins) {
                        var coinsToTrade = Math.Min(coinsMerchant, amount.getAmount());
                        for(int i = 0; i < coinsToTrade; i++) {
                            mMerchantStockAmounts.remove(product, 1);
                            mMerchantTradeAmounts.add(product, 1);
                        }
                        CoreUtils.InvokeMethod("updateUi", __instance);
                        return;
                    }
                }
            }
        }
    }
}
