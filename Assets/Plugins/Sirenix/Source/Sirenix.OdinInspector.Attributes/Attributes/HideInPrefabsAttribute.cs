//-----------------------------------------------------------------------
// <copyright file="HideInPrefabsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;
    using System.ComponentModel;

    /// <summary>
    /// Hides a property if it is drawn from a prefab instance or a prefab asset.
    /// </summary>
    [Obsolete("Use [HideIn(PrefabKind.PrefabAsset | PrefabKind.PrefabInstance)] instead.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class HideInPrefabsAttribute : Attribute
    {
    }
}