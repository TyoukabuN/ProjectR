//-----------------------------------------------------------------------
// <copyright file="DisableInEditorModeExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(DisableInEditorModeAttribute))]
    internal class DisableInEditorModeExamples
    {
        [Title("Disabled in edit mode")]
        [DisableInEditorMode]
        public GameObject A;

        [DisableInEditorMode]
        public Material B;
    }
}
#endif