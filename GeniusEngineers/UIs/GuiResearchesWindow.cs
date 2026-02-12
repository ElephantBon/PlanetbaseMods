using Planetbase;
using UnityEngine;

namespace GeniusEngineers
{
    public class GuiResearchesWindow : GuiWindow
    {
        public GuiResearchesWindow()
            : base(new GuiLabelItem("Research"))
        {
            AddItem("Extra Bot Life", Main.ExtraBotLife, ResourceList.StaticIcons.Bot, "Improve integrity to make bots last longer.");
            AddItem("Extra Vegetable Life", Main.ExtraVegetableLife, ResourceList.StaticIcons.Food, "Increase lifetime of vegetable pads.");
            AddItem("Extra Power Storage", Main.ExtraPowerStorage, ResourceList.StaticIcons.PowerStorage, "Increase capacity of all power collectors.");
            AddItem("Extra Water Storage", Main.ExtraWaterStorage, ResourceList.StaticIcons.Water, "Increase capacity of all water tanks.");
        }

        private void AddItem(string name, float value, Texture2D icon, string description)
        {
            GuiLabelItem guiLabelItem = new GuiLabelItem($"{name}: {value:0.0}%", icon, description);
            guiLabelItem.setEnabled(value > 0);
            mRootItem.addChild(guiLabelItem);
        }

        public override float getWidth()
        {
            return (float)Screen.height * 0.45f;
        }
    }
}
