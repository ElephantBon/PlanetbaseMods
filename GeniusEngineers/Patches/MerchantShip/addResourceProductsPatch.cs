using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;

namespace GeniusEngineers.Patches
{
    [HarmonyPatch(typeof(MerchantShip), "addResourceProducts")]
    public class addResourceProductsPatch
    {
        public static void Postfix(MerchantShip __instance)
        {
            var productAmounts = __instance.getProductAmounts();
            var mProductAmounts = CoreUtils.GetMember<ProductAmounts, Dictionary<string, ProductAmount>>("mProductAmounts", productAmounts);
            if(mProductAmounts == null)
                return;

            var removingProducts = new List<string>();
            foreach(var productAmount in productAmounts) {
                var product = productAmount.getProduct();
                var resourceType = ((ProductResource)product).getResourceType();
                if(resourceType is ResourceExtraBotLife
                || resourceType is ResourceExtraVegetableLife
                || resourceType is ResourceExtraPowerStorage
                || resourceType is ResourceExtraWaterStorage)
                    removingProducts.Add(product.getId());
            }

            foreach(var id in removingProducts)
                mProductAmounts.Remove(id);            
        }
    }
}
