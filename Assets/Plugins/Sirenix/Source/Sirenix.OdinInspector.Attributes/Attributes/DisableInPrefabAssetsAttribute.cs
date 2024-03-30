//-----------------------------------------------------------------------
// <copyright file="DisableInPrefabAssetsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;
    using System.ComponentModel;

    /// <summary>
    /// Disables a property if it is drawn from a prefab asset.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use [DisableIn(PrefabKind.PrefabAsset)] instead.", false)]
    public class DisableInPrefabAssetsAttribute : Attribute
    {
    }
}