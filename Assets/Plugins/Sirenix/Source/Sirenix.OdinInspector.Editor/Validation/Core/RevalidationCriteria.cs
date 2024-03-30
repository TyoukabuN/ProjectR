//-----------------------------------------------------------------------
// <copyright file="RevalidationCriteria.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    public enum RevalidationCriteria
    {
        Always,
        OnValueChange,
        OnValueChangeOrChildValueChange
    }
}
#endif