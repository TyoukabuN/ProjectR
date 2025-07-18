#if  UNITY_EDITOR
using PJR.Core.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace PJR.BlackBoard.Editor.Drawers
{
    public class CacheableFieldDrawer<T> : OdinValueDrawer<CacheableField<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }
    }
}
#endif