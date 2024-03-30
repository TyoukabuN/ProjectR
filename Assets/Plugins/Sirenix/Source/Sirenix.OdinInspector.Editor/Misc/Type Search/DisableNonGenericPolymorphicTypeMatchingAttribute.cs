//-----------------------------------------------------------------------
// <copyright file="DisableNonGenericPolymorphicTypeMatchingAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DisableNonGenericPolymorphicTypeMatchingAttribute : Attribute { }
}
#endif