//-----------------------------------------------------------------------
// <copyright file="InlineButtonExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(InlineButtonAttribute))]
    internal class InlineButtonExamples
    {
        // Inline Buttons:
        [InlineButton("A")]
        public int InlineButton;

        [InlineButton("A")]
        [InlineButton("B", "Custom Button Name")]
        public int ChainedButtons;

        private void A()
        {
            Debug.Log("A");
        }

        private void B()
        {
            Debug.Log("B");
        }
    }
}
#endif