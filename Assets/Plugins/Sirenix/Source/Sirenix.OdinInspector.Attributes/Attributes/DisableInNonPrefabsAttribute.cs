//-----------------------------------------------------------------------
// <copyright file="DisableInNonPrefabsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel;

namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;

    /// <summary>
    /// Disables a property if it is drawn from a non-prefab asset or instance.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [Obsolete("Use [DisableIn(PrefabKind.NonPrefabInstance)] instead.", false)]
    public class DisableInNonPrefabsAttribute : Attribute
    {
    }
}