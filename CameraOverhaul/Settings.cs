using UnityEngine;
using UnityModManagerNet;

namespace CameraOverhaul
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Screen Edge Scrolling Enabled")] public bool ScreenEdgeScrollingEnabled = true;
        [Draw("Min Height")] public float MinHeight = 12;
        [Draw("Max Height")] public float MaxHeight = 120;
        [Draw("Zoom Speed Multiplier")] public float ZoomSpeedMultiplier = 1;
        [Draw("Zoom Z Motion Easing Enabled")] public bool ZoomZMotionEasingEnabled = true;
        [Draw("Zoom Speed Easing Enabled")] public bool ZoomSpeedEasingEnabled = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
}
