//-----------------------------------------------------------------------
// <copyright file="HideMonoScriptScriptableObject.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [HideMonoScript]
    public class HideMonoScriptScriptableObject : ScriptableObject
    {
        public string Value;
    }
}
#endif