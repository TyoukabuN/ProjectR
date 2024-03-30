//-----------------------------------------------------------------------
// <copyright file="ButtonGroupExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    [AttributeExample(typeof(ButtonGroupAttribute))]
    internal class ButtonGroupExamples
    {
        [ButtonGroup]
        private void A()
        {
        }

        [ButtonGroup]
        private void B()
        {
        }

        [ButtonGroup]
        private void C()
        {
        }

        [ButtonGroup]
        private void D()
        {
        }

        [Button(ButtonSizes.Large)]
        [ButtonGroup("My Button Group")]
        private void E()
        {
        }

        [GUIColor(0, 1, 0)]
        [ButtonGroup("My Button Group")]
        private void F()
        {
        }
    }
}
#endif