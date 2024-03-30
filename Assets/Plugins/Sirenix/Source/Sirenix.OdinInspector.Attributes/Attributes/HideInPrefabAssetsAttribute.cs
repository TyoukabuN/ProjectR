//-----------------------------------------------------------------------
// <copyright file="HideInPrefabAssetsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;
    using System.ComponentModel;

    /// <summary>
    /// Hides a property if it is drawn from a prefab asset.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [Obsolete("Use [HideIn(PrefabKind.PrefabAsset)] instead.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class HideInPrefabAssetsAttribute : Attribute
    {
    }
}