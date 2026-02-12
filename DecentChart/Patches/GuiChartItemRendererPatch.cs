using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecentChart.Patches
{
    [HarmonyPatch(typeof(GuiChartItemRenderer))]
    internal class GuiChartItemRendererPatch
    {
        [HarmonyPatch(nameof(GuiChartItemRenderer.render))]
        [HarmonyPrefix]
        public static bool render(GuiChartItemRenderer __instance, GuiRenderer guiRenderer, GuiWindow window, GuiChartItem item, float x, float y)
        {
            Dictionary<string, CountHistory> data = item.getData();
            float num = (float)GuiStyles.getIconMargin() * 3f;
            float num2 = GetCustomWidth(item, window) - num * 2f;
            float height = item.getHeight();
            x += (float)GuiStyles.getIconMargin();
            GUI.Box(new Rect(x, y, num2, height), string.Empty, guiRenderer.getBoxStyle());
            int num3 = 1;
            int num4 = 0;
            foreach(KeyValuePair<string, CountHistory> item2 in data) {
                if(item.isStatEnabled(item2.Key)) {
                    int max = item2.Value.getMax();
                    if(max > num3) {
                        num3 = max;
                    }

                    if(item2.Value.getList().Count > num4) {
                        num4 = item2.Value.getList().Count;
                    }
                }
            }

            num3 = __instance.calculateMax(num3);
            int num5 = num3 / 2;
            string text = ((num3 >= 1000) ? (num3 / 1000 + "K") : num3.ToString());
            string text2 = ((num5 >= 1000) ? (num5 / 1000 + "K") : num5.ToString());
            GUIStyle labelStyle = guiRenderer.getLabelStyle();
            float x2 = labelStyle.CalcSize(new GUIContent(text)).x;
            float x3 = labelStyle.CalcSize(new GUIContent(text2)).x;
            Vector2 vector = labelStyle.CalcSize(new GUIContent("0"));
            float num6 = y + item.getHeight() - vector.y;
            GUI.Label(new Rect(x - x2, y, 100f, 100f), text, labelStyle);
            GUI.Label(new Rect(x - x3, y + item.getHeight() * 0.5f - vector.y * 0.5f, 100f, 100f), text2, labelStyle);
            GUI.Label(new Rect(x - vector.x, num6, 100f, 100f), "0", labelStyle);
            float num7 = 8f;
            labelStyle.alignment = TextAnchor.UpperCenter;
            string text3 = StringList.get("charts_day_initial");
            float y2 = labelStyle.CalcSize(new GUIContent("100")).y;
            GUI.Label(new Rect(x + num2 * 0.25f - 20f, num6 + y2, 40f, 25f), Mathf.RoundToInt(num7 * 0.75f) + text3, labelStyle);
            GUI.Label(new Rect(x + num2 * 0.5f - 20f, num6 + y2, 40f, 25f), Mathf.RoundToInt(num7 * 0.5f) + text3, labelStyle);
            GUI.Label(new Rect(x + num2 * 0.75f - 20f, num6 + y2, 40f, 25f), Mathf.RoundToInt(num7 * 0.25f) + text3, labelStyle);
            y = (float)Screen.height - y - item.getHeight();
            CoreUtils.InvokeMethod("beginDrawLine", __instance, Color.gray);
            float num8 = (float)GuiStyles.getIconMargin() * 0.5f;
            float size = item.getHeight() * 0.0025f;
            float num9 = num2 - num8 * 2f;
            float num10 = height - num8 * 2f;
            float num11 = x + num8;
            float num12 = y + num8;
            for(float num13 = 0.25f; num13 < 1f; num13 += 0.25f) {
                CoreUtils.InvokeMethod("drawSegment", __instance, new Vector2(num11, num12 + height * num13), new Vector2(num11 + num9, num12 + height * num13), size);
            }

            for(float num14 = 0.25f; num14 < 1f; num14 += 0.25f) {
                CoreUtils.InvokeMethod("drawSegment", __instance, new Vector2(num11 + num9 * num14, num12), new Vector2(num11 + num9 * num14, num12 + num10), size);
            }

            CoreUtils.InvokeMethod("endDrawLine", __instance);
            int num15 = 0;
            foreach(KeyValuePair<string, CountHistory> item3 in data) {
                if(item.isStatEnabled(item3.Key)) {
                    Color color = GuiStyles.UiColorMain;
                    ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.find(item3.Key);
                    if(resourceType != null) {
                        color = resourceType.getStatsColor();
                    }

                    Specialization specialization = TypeList<Specialization, SpecializationList>.find(item3.Key);
                    if(specialization != null) {
                        color = specialization.getColor() * 2f;
                    }

                    CoreUtils.InvokeMethod("renderHistory", __instance, color, num, num3, window, item, x, y, item3.Value);
                    num15++;
                }
            }

            return false;
        }

        [HarmonyPatch("renderHistory")]
        [HarmonyPrefix]
        public static bool renderHistory(GuiChartItemRenderer __instance, Color color, float margin, float max, GuiWindow window, GuiChartItem item, float x, float y, CountHistory history)
        {
            float num = (float)GuiStyles.getIconMargin() * 0.5f;
            List<int> list = history.getList();
            int count = list.Count;
            CoreUtils.InvokeMethod("beginDrawLine", __instance, color);
            float num2 = GetCustomWidth(item, window) - margin * 2f - num * 2f;
            float num3 = item.getHeight() - num * 2f;
            float num4 = num2 / (float)(count - 1);
            float num5 = num3 / max;
            float size = num3 * 0.005f;
            Vector2 vector = new Vector2(x + num, y + num);
            for(int i = 0; i < list.Count - 1; i++) {
                int num6 = Mathf.Max(list[i], 0);
                int num7 = Mathf.Max(list[i + 1], 0);
                Vector2 a = vector + new Vector2((float)i * num4, (float)num6 * num5);
                Vector2 b = vector + new Vector2((float)(i + 1) * num4, (float)num7 * num5);
                CoreUtils.InvokeMethod("drawSegment", __instance, a, b, size);
            }

            CoreUtils.InvokeMethod("endDrawLine", __instance);
            return false;
        }

        private static float GetCustomWidth(GuiChartItem item, GuiWindow window)
        {
            if(item is GuiChartItem) {
                var mObjectsToShow = CoreUtils.GetMember<GuiChartItem, Dictionary<string, RefBool>>("mObjectsToShow", item);
                if(mObjectsToShow == null || mObjectsToShow.Count == 0 || mObjectsToShow.First().Key == "PowerStorage")
                    return (float)Screen.height * 0.85f;
                else
                    return (float)Screen.height * 0.85f * 0.55f;
            }
            else
                return window.getWidth();
        }
    }
}
