using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace PJR.BlackBoard.Editor.Drawers
{
    public class BoardValueReferenceAttributeDrawer : OdinAttributeDrawer<BoardValueReferenceAttribute>
    {
        private IPropertyValueEntry<int> valueEntry;
        protected override void Initialize()
        {
            base.Initialize();
            valueEntry = this.Property.TryGetTypedValueEntry<int>();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (valueEntry.WeakSmartValue != null)
            {
                CallNextDrawer(label);
                return;
            }

            if (GUILayout.Button("获取黑板值", GUIStyles.MiniRichTextButton))
            {
            }
        }
    }
}