//-----------------------------------------------------------------------
// <copyright file="DisableInAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Sirenix.OdinInspector
{
#pragma warning disable

    /// <summary>
    /// Disables a member based on which type of a prefab and instance it is in. 
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class DisableInAttribute : Attribute
    {
        public PrefabKind PrefabKind;

        public DisableInAttribute(PrefabKind prefabKind)
        {
            this.PrefabKind = prefabKind;
        }
    }
}