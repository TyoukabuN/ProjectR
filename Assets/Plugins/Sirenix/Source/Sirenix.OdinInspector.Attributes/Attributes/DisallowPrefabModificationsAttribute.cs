//-----------------------------------------------------------------------
// <copyright file="DisallowModificationsInAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;

    /// <summary>
    /// DisallowModificationsIn disables / grays out members, preventing modifications from being made and enables validation,
    /// providing error messages in case a modification was made prior to introducing the attribute.
    /// </summary>
    /// <seealso cref="DisableInAttribute"/>
    /// <seealso cref="HideInAttribute"/>
    /// <seealso cref="RequiredAttribute"/>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class DisallowModificationsInAttribute : Attribute
    {
        public PrefabKind PrefabKind;

        public DisallowModificationsInAttribute(PrefabKind kind)
        {
            this.PrefabKind  = kind;
        }
    }
}