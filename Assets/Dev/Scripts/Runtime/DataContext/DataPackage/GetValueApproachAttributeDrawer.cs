#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector.Editor;

namespace PJR.Dev.Game.DataContext
{
    public class GetValueApproachAttributeDrawer : OdinAttributeDrawer<GetValueApproachAttribute>
    {
        public const string FloatValueColor = "#FFE100";
        public const string IntValueColor = "#ADD8E6";
        public const string BoolValueColor = "#98FB98";
        
        private IPropertyValueEntry<int> valueEntry;
        private static GUIContent FloatContent => floatContent ??= new GUIContent($"<color={FloatValueColor}>F</color>", "Float值. 点击切换为数据类型");
        private static GUIContent IntContent => intContent ??= new GUIContent($"<color={IntValueColor}>I</color>", "Int值. 点击切换为数据类型");
        private static GUIContent BoolContent => boolContent ??= new GUIContent($"<color={BoolValueColor}>B</color>", "Bool值. 点击切换为数据类型");
        
        private static GUIContent floatContent;
        private static GUIContent intContent;
        private static GUIContent boolContent;
        

        protected override void Initialize()
        {
            base.Initialize();
            valueEntry = this.Property.TryGetTypedValueEntry<int>();
        }

        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ValueEntry != null && property.ValueEntry.TypeOfValue == typeof(int);
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (valueEntry == null) return;
            GUIContent c;
            var approach = valueEntry.SmartValue;

            if (approach == 0)
                c = FloatContent;
            else if (approach == 1)
                c = IntContent;
            else
                c = BoolContent;

            if (GUILayout.Button(c, MiniRichTextButton, GUILayout.Width(20f)))
            {
                //todo:被Dictionary<T>包住的时候出问题
                valueEntry.SmartValue++;
                valueEntry.SmartValue %= (int)DataValue.EValueType.Length;
            }
        }
        
        private static GUIStyle _minirichTextButton;

        public static GUIStyle MiniRichTextButton
        {
            get
            {
                if (_minirichTextButton == null)
                {
                    _minirichTextButton = new GUIStyle(EditorStyles.miniButton);
                    _minirichTextButton.richText = true;
                    _minirichTextButton.padding = new RectOffset(0, 0, 0, 0);
                    _minirichTextButton.alignment = TextAnchor.MiddleCenter;
                }

                return _minirichTextButton;
            }
        }
    }
}
#endif
