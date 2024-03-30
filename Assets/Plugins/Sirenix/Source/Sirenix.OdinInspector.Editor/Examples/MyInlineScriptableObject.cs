//-----------------------------------------------------------------------
// <copyright file="MyInlineScriptableObject.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    public class MyInlineScriptableObject : ScriptableObject
    {
        [ShowInInlineEditors]
        public string ShownInInlineEditor;

        [HideInInlineEditors]
        public string HiddenInInlineEditor;
    }
}
#endif