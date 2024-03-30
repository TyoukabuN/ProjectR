//-----------------------------------------------------------------------
// <copyright file="ShowInAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Sirenix.OdinInspector
{
#pragma warning disable

    /// <summary>
    /// Shows a member based on which type of a prefab and instance it is in. 
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class ShowInAttribute : Attribute
    {
        public PrefabKind PrefabKind;

        public ShowInAttribute(PrefabKind prefabKind)
        {
            this.PrefabKind = prefabKind;
        }
    }
}