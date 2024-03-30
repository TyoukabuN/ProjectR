//-----------------------------------------------------------------------
// <copyright file="HideInTablesAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;

    /// <summary>
    /// The HideInTables attribute is used to prevent members from showing up as columns in tables drawn using the <see cref="TableListAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class HideInTablesAttribute : Attribute
    {
    }
}