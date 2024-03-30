//-----------------------------------------------------------------------
// <copyright file="DisableInPrefabsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;
    using System.ComponentModel;

    /// <summary>
    /// Disables a property if it is drawn from a prefab asset or a prefab instance.
    /// </summary>
    [Obsolete("Use [DisableIn(PrefabKind.PrefabAsset | PrefabKind.PrefabInstance)] instead.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class DisableInPrefabsAttribute : Attribute
    {
    }
}