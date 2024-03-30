using System;
using UnityEngine;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable
    [AttributeExample(typeof(FoldoutBoxPropertyAttribute))]
    // [ShowOdinSerializedPropertiesInInspector]
    internal class FoldoutBoxPropertyExample
    {
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [Indent]
        public WrapScript w;

        [OnInspectorInit]
        private void CreateData()
        {
            w = ExampleHelper.GetScriptableObject<WrapScript>("w");
        }

        public class WrapScript : ScriptableObject
        {
            [Title("确定类型字段")]
            [ShowInInspector]
            public ClassA Field;
            [FoldoutBoxProperty]
            [ShowInInspector]
            public ClassA FieldBoxed;
            
            
            [Title("多态类型字段")]
            [ShowInInspector]
            public AbstractClass PolyField;
            [FoldoutBoxProperty][ShowInInspector]
            public AbstractClass PolyFieldBoxed;
        }

        [Serializable]
        public abstract class AbstractClass
        {
            public int baseField;
        }
        [Serializable]
        public class ClassA : AbstractClass
        {
            public int AField;
        }
        [Serializable]
        public class ClassB : AbstractClass
        {
            public int BField;
        }
    }
}
#endif
