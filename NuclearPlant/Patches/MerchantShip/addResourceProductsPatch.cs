using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;
using static Planetbase.LandingShip;

namespace NuclearPlant.Patches
{
    [HarmonyPatch(typeof(MerchantShip), "addResourceProducts")]
    internal class addResourceProductsPatch
    {
        public static bool Prefix(MerchantShip __instance)
        {
            var mSize = CoreUtils.GetMember<MerchantShip, Size>("mSize", __instance);
            var mMerchantCategory = __instance.getCategory();

            List<ResourceType> list = new List<ResourceType>();
            List<float> list2 = new List<float>();
            List<ResourceType> list3 = TypeList<ResourceType, ResourceTypeList>.get();
            int num = ((mSize != 0) ? 300 : 100);
            num = num * ((mMerchantCategory != MerchantCategory.Count) ? 150 : 75) / 100;
            if(mMerchantCategory == MerchantCategory.Count) {
                for(int i = 0; i < list3.Count; i++) {
                    ResourceType resourceType = list3[i];

                    // Remove ResourcePower from merchant
                    if(resourceType is ResourcePower)
                        continue;

                    if(Random.Range(0, 2) == 0 || resourceType.hasFlag(2)) {
                        list.Add(resourceType);
                        list2.Add(Random.Range(0.8f, 1.2f) / (float)resourceType.getValue());
                    }
                }
            }
            else {
                for(int j = 0; j < list3.Count; j++) {
                    ResourceType resourceType2 = list3[j];
                    if(resourceType2.getMerchantCategory() == mMerchantCategory || resourceType2.hasFlag(2)) {
                        list.Add(resourceType2);
                        list2.Add(Random.Range(0.5f, 1.5f) / (float)resourceType2.getValue());
                    }
                }
            }

            float num2 = 0f;
            for(int k = 0; k < list2.Count; k++) {
                ResourceType resourceType3 = list[k];
                if(!resourceType3.hasFlag(2)) {
                    num2 += list2[k];
                }
            }

            for(int l = 0; l < list2.Count; l++) {
                ResourceType resourceType4 = list[l];
                if(!resourceType4.hasFlag(2)) {
                    list2[l] /= num2;
                }
            }

            int num3 = __instance.getMaxCargoSpace() / 2;
            int num4 = num3 * 15 / 100;
            num3 += Random.Range(-num4, num4);
            for(int m = 0; m < list2.Count; m++) {
                ResourceType resourceType5 = list[m];
                if(resourceType5.hasFlag(2)) {
                    __instance.getProductAmounts().add(new ProductResource(resourceType5), Random.Range(num / 2, num + 1));
                    continue;
                }

                int max = Mathf.Max(1, num / resourceType5.getValue());
                __instance.getProductAmounts().add(new ProductResource(resourceType5), Mathf.Clamp(Mathf.RoundToInt((float)num3 * list2[m]), 1, max));
            }

            return false;
        }
    }
}
