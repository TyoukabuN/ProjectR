//-----------------------------------------------------------------------
// <copyright file="IApplyableResolver.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    public interface IApplyableResolver
    {
        bool ApplyChanges();
    }
}
#endif