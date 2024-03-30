using UnityEngine;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable
    [AttributeExample(typeof(HideObjectExpandedAttribute))]
    // [ShowOdinSerializedPropertiesInInspector]
    internal class HideObjectExpandedExamples
    {
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [Indent]
        public WrapScript w;

        [OnInspectorInit]
        private void CreateData()
        {
            w = ExampleHelper.GetScriptableObject<WrapScript>("w");
            w.config1 = ExampleHelper.GetScriptableObject<ExampleTransform>("Boxed");
            w.config2 = ExampleHelper.GetScriptableObject<ExampleTransform>("Foldout");
        }

        public class WrapScript : ScriptableObject
        {
            [Title("No [HideObjectExpanded] attritube")]
            public ExampleTransform config1;
        
            [Title("Add [HideObjectExpanded] attritube")]
            [HideObjectExpanded]
            public ExampleTransform config2;
        }
    }
}
#endif