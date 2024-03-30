//-----------------------------------------------------------------------
// <copyright file="DisabledInInlineEditorScriptableObject.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    public class DisabledInInlineEditorScriptableObject : ScriptableObject
    {
        public string AlwaysEnabled;

        [DisableInInlineEditors]
        public string DisabledInInlineEditor;
    }
}
#endif