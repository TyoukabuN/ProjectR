//-----------------------------------------------------------------------
// <copyright file="RequiredInAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;

    /// <summary>
    /// Makes a member required based on which type of a prefab and instance it is in. 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class RequiredInAttribute : System.Attribute
    {
        public string ErrorMessage;
        public PrefabKind PrefabKind;

        public RequiredInAttribute(PrefabKind kind)
        {
            this.PrefabKind = kind;
        }
    }
}