//-----------------------------------------------------------------------
// <copyright file="DoesNotSupportPrefabModificationsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    internal sealed class DoesNotSupportPrefabModificationsAttribute : Attribute
    {
    }
}
#endif