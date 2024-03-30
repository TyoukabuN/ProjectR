//-----------------------------------------------------------------------
// <copyright file="ValidationResultType.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    public enum ValidationResultType
    {
        IgnoreResult,
        Valid,
        Error,
        Warning,
    }
}
#endif