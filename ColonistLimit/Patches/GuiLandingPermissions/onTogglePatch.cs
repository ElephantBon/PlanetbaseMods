using HarmonyLib;
using Planetbase;
using System.Linq;

namespace ColonistLimit.Patches
{
    [HarmonyPatch(typeof(GuiLandingPermissions), nameof(GuiLandingPermissions.onToggle))]
    internal class onTogglePatch
    {
        public static void Postfix(GuiLandingPermissions __instance, object parameter)
        {
            var mRootItem = __instance.getRootItem();
            var guiSectionItem = mRootItem.getChildren().First();
            var children = guiSectionItem.getChildren();
            var mColonistsLabelItem = children.First();

            if(!(mColonistsLabelItem is GuiLabelItem))
                return;

            GuiRowItem guiRowItem2 = new GuiRowItem(2);

            // Replace colonist checkbox with rowItem            
            children.Remove(mColonistsLabelItem);
            children.Insert(0, guiRowItem2);
            guiRowItem2.addChild(mColonistsLabelItem);

            // Limit selector
            GuiAmountSelector guiAmountSelector = new GuiAmountSelector(0, 10000, 1, Main.colonistLimit, onChange, 0);
            guiAmountSelector.setHeight(GuiStyles.getIconSizeSmall());
            guiAmountSelector.setTooltip("Permission for colonists will be disabled automatically once reached limit");
            guiRowItem2.addChild(guiAmountSelector);
        }

        public static void onChange(object parameter)
        {
            //int num = 0;
            //int count = SpecializationList.getColonistSpecializations().Count;
            //foreach(Specialization colonistSpecialization in SpecializationList.getColonistSpecializations()) {
            //    num += mLandingPermissions.getSpecializationPercentage(colonistSpecialization).get();
            //}

            //int num2 = mSpecializationAmountSelectors.IndexOf((GuiAmountSelector)parameter);
            //int num3 = num - 100;
            //int num4 = (num2 + 1) % count;
            //while(num3 > 0) {
            //    GuiAmountSelector guiAmountSelector = mSpecializationAmountSelectors[num4];
            //    int current2 = guiAmountSelector.getCurrent();
            //    if(current2 > 0) {
            //        int num5 = Mathf.Min(num3, current2);
            //        num3 -= num5;
            //        guiAmountSelector.setCurrent(current2 - num5);
            //    }

            //    num4 = (num4 + 1) % count;
            //}

            //while(num3 < 0) {
            //    GuiAmountSelector guiAmountSelector2 = mSpecializationAmountSelectors[num4];
            //    int current3 = guiAmountSelector2.getCurrent();
            //    if(current3 < 100) {
            //        int num6 = Mathf.Min(-num3, 100 - current3);
            //        num3 += num6;
            //        guiAmountSelector2.setCurrent(current3 + num6);
            //    }

            //    num4 = (num4 + 1) % count;
            //}

            //for(int i = 0; i < mSpecializationAmountSelectors.Count; i++) {
            //    GuiAmountSelector guiAmountSelector3 = mSpecializationAmountSelectors[i];
            //    Specialization specialization = SpecializationList.getColonistSpecializations()[i];
            //    guiAmountSelector3.setTooltip(StringList.get("tooltip_specialization_target", guiAmountSelector3.getCurrent() + "%", specialization.getNamePlural()));
            //}
        }
    }
}
