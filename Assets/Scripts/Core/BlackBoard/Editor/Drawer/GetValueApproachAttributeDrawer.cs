#if  UNITY_EDITOR
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace PJR.BlackBoard.Editor.Drawers
{
    public class GetValueApproachAttributeDrawer : OdinAttributeDrawer<GetValueApproachAttribute>
    {
        public const string LocalValueColor = "#98FB98";
        public const string BlackboardValueColor = "#ADD8E6";
        public const string EvaluationValueColor = "#ADD8E6";
        
        private IPropertyValueEntry<int> valueEntry;
        private static GUIContent localContent;
        private static GUIContent LocalContent => localContent ??= new GUIContent($"<color={LocalValueColor}>_</color>", "本地值. 点击切换为参数输入");
        
        private static GUIContent boardContent;
        private static GUIContent BoardContent => boardContent ??= new GUIContent($"<color={BlackboardValueColor}>[-]</color>", "黑板值. 点击切换为参数输入");
        
        // private static GUIContent evContent;
        // private static GUIContent EvContent => evContent ??= new GUIContent($"<color={BlackboardValueColor}>[Ev]</color>", "求值. 点击切换为本地值");

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
                c = LocalContent;
            else if (approach == 1)
                c = BoardContent;
            else
                c = LocalContent;

            if (GUILayout.Button(c, GUIStyles.MiniRichTextButton, GUILayout.Width(22f)))
            {
                valueEntry.SmartValue++;
                valueEntry.SmartValue %= (int)EGetValueApproach.ByEvaluation;
            }
        }
    }
}
#endif