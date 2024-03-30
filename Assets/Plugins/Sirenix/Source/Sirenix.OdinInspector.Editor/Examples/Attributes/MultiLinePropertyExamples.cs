//-----------------------------------------------------------------------
// <copyright file="MultiLinePropertyExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(MultilineAttribute))]
    [AttributeExample(typeof(MultiLinePropertyAttribute))]
    internal class MultiLinePropertyExamples
    {
        [Multiline(10)]
        public string UnityMultilineField = "";

        [Title("Wide Multiline Text Field", bold: false)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string WideMultilineTextField = "";

        [InfoBox("Odin supports properties, but Unity's own Multiline attribute only works on fields.")]
        [ShowInInspector]
        [MultiLineProperty(10)]
        public string OdinMultilineProperty { get; set; }
    }
}
#endif