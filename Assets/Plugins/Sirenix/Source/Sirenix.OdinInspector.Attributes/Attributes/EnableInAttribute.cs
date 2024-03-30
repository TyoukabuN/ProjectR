//-----------------------------------------------------------------------
// <copyright file="EnableInAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Sirenix.OdinInspector
{
#pragma warning disable

    /// <summary>
    /// Enables a member based on which type of a prefab and instance it is. 
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class EnableInAttribute : Attribute
    {
        public PrefabKind PrefabKind;

        public EnableInAttribute(PrefabKind prefabKind)
        {
            this.PrefabKind = prefabKind;
        }
    }
}