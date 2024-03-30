//-----------------------------------------------------------------------
// <copyright file="CustomContextMenuExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(CustomContextMenuAttribute))]
    internal class CustomContextMenuExamples
    {
        [InfoBox("A custom context menu is added on this property. Right click the property to view the custom context menu.")]
        [CustomContextMenu("Say Hello/Twice", "SayHello")]
        public int MyProperty;

        private void SayHello()
        {
            Debug.Log("Hello Twice");
        }
    }
}
#endif