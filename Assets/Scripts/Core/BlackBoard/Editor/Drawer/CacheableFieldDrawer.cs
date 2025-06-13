using NPOI.SS.Formula.Functions;
using PJR.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace PJR.BlackBoard.Editor.Drawers
{
    public class CacheableFieldDrawer : OdinValueDrawer<CacheableField<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }
    }
}