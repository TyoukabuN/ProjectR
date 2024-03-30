using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable
    [AttributeExample(typeof(DrawListElementIndividualAttribute))]
    // [ShowOdinSerializedPropertiesInInspector]
    internal class DrawListElementIndividualExamples
    {

        [Title("No [DrawListElementIndividual] attritube")]
        [ListDrawerSettings(Expanded = true)]
        public List<SomeStruct> list1;
        
        [Title("Add [DrawListElementIndividual] attritube")]
        [DrawListElementIndividual]
        public List<SomeStruct> list2;

        [OnInspectorInit]
        private void CreateData()
        {
            list1 = new List<SomeStruct>();
            list1.Add(new SomeStruct{str = "Element 1"});
            list1.Add(new SomeStruct{str = "Element 2"});
            list2 = new List<SomeStruct>(list1);
        }
        
        [Serializable]
        public struct SomeStruct
        {
            [HorizontalGroup("1")]
            [HideLabel]
            public string str;
            [VerticalGroup("1/2")]
            public KeyCode key;
            [VerticalGroup("1/2")]
            public int index;
        }
    }
}
#endif