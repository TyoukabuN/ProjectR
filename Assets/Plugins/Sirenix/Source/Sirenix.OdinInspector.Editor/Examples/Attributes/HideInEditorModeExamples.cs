//-----------------------------------------------------------------------
// <copyright file="HideInEditorModeExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    [AttributeExample(typeof(HideInEditorModeAttribute))]
    internal class HideInEditorModeExamples
    {
        [Title("Hidden in editor mode")]
        [HideInEditorMode]
        public int C;

        [HideInEditorMode]
        public int D;
    }
}
#endif