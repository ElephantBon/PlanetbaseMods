using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecentChart.Patches
{
    [HarmonyPatch(typeof(GuiChartsWindow))]
    internal class GuiChartsWindowPatch
    {
        [HarmonyPatch("updateUI")]
        [HarmonyPostfix]
        public static void updateUI(GuiChartsWindow __instance)
        {
            var mShowObjects = CoreUtils.GetMember<GuiChartsWindow, Dictionary<string, RefBool>>("mShowObjects");
            var activeObjects = mShowObjects.Where(x => x.Value.get() == true).ToDictionary(x => x.Key, x => x.Value);
            var mRootItem = __instance.getRootItem();

            // Reserve first chart for power storage
            if(AddPowerChart(mRootItem, activeObjects)) {
                AddRow(mRootItem);

                // Reserve second chart for power storage
                if(AddWaterChart(mRootItem, activeObjects)) 
                    AddRestCharts(mRootItem, activeObjects);
                else {
                    // Charts share two cells
                    var groups = GroupingObjects(activeObjects, 2);
                    foreach(var group in groups)
                        AddGroupCharts(mRootItem, group);
                }
            }
            else {
                // All charts share 4 cells
                mRootItem.removeLastChild();    // Remove original chart
                var groups = GroupingObjects(activeObjects, 4);
                foreach(var group in groups)
                    AddGroupCharts(mRootItem, group);

                // Reserve empty space
                if(groups.Length == 0) {
                    AddRow(mRootItem);
                    AddRow(mRootItem);
                }
                else
                if(groups.Length <= 2)
                    AddRow(mRootItem);
            }
        }

        private static void AddGroupCharts(GuiLabelItem mRootItem, Dictionary<string, RefBool> group)
        {
            var lastItem = mRootItem.getLastChild();
            if(!(lastItem is GuiRowItem) || lastItem.getChildren().Count >= ((GuiRowItem)lastItem).getColumns()) {
                // Add new row
                lastItem = AddRow(mRootItem);
            }
            var rowItem = (GuiRowItem)lastItem;
            rowItem.addChild(new GuiChartItem(Singleton<StatsCollector>.getInstance().getData(), group));
        }

        /// <summary> Seperate dictionary into groups by max history value </summary>
        private static Dictionary<string, RefBool>[] GroupingObjects(Dictionary<string, RefBool> activeObjects, int groupCount)
        {
            if(activeObjects.Count <= groupCount) {
                var groups = new Dictionary<string, RefBool>[activeObjects.Count];
                for(int i = 0; i < activeObjects.Count; i++) {
                    var activeObject = activeObjects.ElementAt(i);
                    groups[i] = new Dictionary<string, RefBool>() { { activeObject.Key, activeObject.Value } };
                }
                return groups;
            }
            else {
                // Calculate max of histories
                var data = Singleton<StatsCollector>.getInstance().getData();
                var objects = new Dictionary<KeyValuePair<string, RefBool>, int>();
                var total = 0;
                for(int i = 0; i < activeObjects.Count; i++) {
                    var activeObject = activeObjects.ElementAt(i);
                    var objValue = data[activeObject.Key].getMax();
                    objects.Add(activeObject, objValue);
                    total += objValue;
                }

                // Sort
                objects = objects.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                // Grouping
                return Grouping(objects, groupCount);
            }
        }

        private static Dictionary<string, RefBool>[] Grouping(Dictionary<KeyValuePair<string, RefBool>, int> objects, int groupCount)
        {
            if(objects.Count == 1) {
                var group = new Dictionary<string, RefBool>();
                AddToGroup(group, objects.First());
                return new Dictionary<string, RefBool>[] { group };
            }

            var total = objects.Sum(x => x.Value);
            var avg = total / objects.Count;
            if(groupCount > 2) {
                var objects1 = objects.Where(x => x.Value < avg).ToDictionary(x => x.Key, x => x.Value);
                var groups1 = Grouping(objects1, groupCount / 2);

                var objects2 = objects.Skip(objects1.Count()).ToDictionary(x => x.Key, x => x.Value);
                var groups2 = Grouping(objects2, groupCount / 2);

                return groups1.Union(groups2).ToArray();
            }
            else {
                var group1 = new Dictionary<string, RefBool>();
                var group2 = new Dictionary<string, RefBool>();
                AddToGroup(group1, objects.First());
                AddToGroup(group2, objects.Last());

                for(int i = 1; i < objects.Count - 1; i++) {
                    var obj = objects.ElementAt(i);
                    var objValue = obj.Value;
                    if(objValue < avg)
                        AddToGroup(group1, obj);
                    else
                        AddToGroup(group2, obj);
                }
                return new Dictionary<string, RefBool>[] { group1, group2 };
            }
        }

        private static void AddToGroup(Dictionary<string, RefBool> group, KeyValuePair<KeyValuePair<string, RefBool>, int> obj)
        {
            group.Add(obj.Key.Key, obj.Key.Value);
        }

        private static bool AddPowerChart(GuiLabelItem mRootItem, Dictionary<string, RefBool> activeObjects)
        {
            var powerDictionary = activeObjects.Where(x => x.Key == "PowerStorage").ToDictionary(x => x.Key, x => x.Value);
            if(powerDictionary.Count == 1) {
                activeObjects.Remove("PowerStorage");
                CoreUtils.SetMember("mObjectsToShow", (GuiChartItem)mRootItem.getLastChild(), powerDictionary);
                return true;
            }
            else
                return false;
        }

        private static bool AddWaterChart(GuiLabelItem mRootItem, Dictionary<string, RefBool> activeObjects)
        {
            var waterDictionary = activeObjects.Where(x => x.Key == "WaterStorage").ToDictionary(x => x.Key, x => x.Value);
            if(waterDictionary.Count == 1) {
                activeObjects.Remove("WaterStorage");

                var item = (GuiRowItem)mRootItem.getLastChild();
                item.addChild(new GuiChartItem(Singleton<StatsCollector>.getInstance().getData(), waterDictionary));

                return true;
            }
            else
                return false;
        }

        private static void AddRestCharts(GuiLabelItem mRootItem, Dictionary<string, RefBool> activeObjects)
        {
            var item = (GuiRowItem)mRootItem.getLastChild();
            item.addChild(new GuiChartItem(Singleton<StatsCollector>.getInstance().getData(), activeObjects));
        }

        private static GuiRowItem AddRow(GuiWindowItem mRootItem)
        {
            var rowItem = new GuiRowItem(2);
            rowItem.setHeight((float)Screen.height * 0.3f);
            mRootItem.addChild(rowItem);
            return rowItem;
        }
        private static Dictionary<string, RefBool> NewDictionaryObject(KeyValuePair<string, RefBool> keyValuePair)
        {
            return new Dictionary<string, RefBool>() { { keyValuePair.Key, keyValuePair.Value } };
        }
        private static GuiWindowItem CreateOneChart(Dictionary<string, RefBool> activeObjects, int index)
        {
            var activeObject = activeObjects.ElementAt(index);
            return new GuiChartItem(Singleton<StatsCollector>.getInstance().getData(), NewDictionaryObject(activeObject));
        }
        private static GuiWindowItem CreateRestChart(Dictionary<string, RefBool> activeObjects, int index)
        {
            var restObjects = activeObjects.Skip(index).ToDictionary(x => x.Key, x => x.Value);
            return new GuiChartItem(Singleton<StatsCollector>.getInstance().getData(), restObjects);
        }

        [HarmonyPatch(nameof(GuiChartsWindow.onClick))]
        [HarmonyPostfix]
        public static void onClick(GuiChartsWindow __instance, object parameter)
        {
            CoreUtils.InvokeMethod("updateUI", __instance);
        }

        // Wierd causing window width too small
        //[HarmonyPatch(nameof(GuiChartsWindow.getWidth))]
        //[HarmonyPrefix]
        //public static bool getWidth(float __result, GuiChartsWindow __instance)
        //{
        //    //__result = (float)Screen.height * 0.85f;
        //    __result = 2000f;
        //    return false;
        //}
    }
}
