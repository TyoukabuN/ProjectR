#if UNITY_EDITOR

using UnityEngine;
using Sirenix.OdinInspector.Editor;

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

#endif
