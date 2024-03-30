//-----------------------------------------------------------------------
// <copyright file="OmitFromPrefabModificationPathsAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class OmitFromPrefabModificationPathsAttribute : Attribute
    {
    }
}
#endif