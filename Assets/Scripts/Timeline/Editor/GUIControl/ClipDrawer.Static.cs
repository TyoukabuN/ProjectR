using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class ClipDrawer
    {
        public static Color GetClipBgColor(bool selected)
        {
            return selected
                ? Styles.Instance.customSkin.clipSelectedBckg
                : Styles.Instance.customSkin.clipBckg;
        }

        public static Color GetClipBorderColor(bool selected)
        {
            return selected
                ? Color.white
                : Styles.Instance.customSkin.clipBorderColor;
        }
    }
}