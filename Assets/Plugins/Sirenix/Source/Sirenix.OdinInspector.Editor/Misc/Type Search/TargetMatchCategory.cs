//-----------------------------------------------------------------------
// <copyright file="TargetMatchCategory.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    using System;

    [Flags]
    public enum TargetMatchCategory
    {
        Value = 1,
        Attribute = 1 << 1,
        All = Value | Attribute,
    }
}
#endif