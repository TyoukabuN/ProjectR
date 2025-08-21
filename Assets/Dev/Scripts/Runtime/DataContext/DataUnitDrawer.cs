using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace PJR.Dev.Game.DataContext
{
    public class DataUnitDrawer : OdinValueDrawer<DataValue>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }
    }
}