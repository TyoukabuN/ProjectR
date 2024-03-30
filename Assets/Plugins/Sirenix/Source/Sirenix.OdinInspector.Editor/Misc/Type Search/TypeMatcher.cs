//-----------------------------------------------------------------------
// <copyright file="TypeMatcher.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    using System;

    public abstract class TypeMatcher
    {
        public abstract string Name { get; }
        public abstract Type Match(Type[] targets, ref bool stopMatching);
    }
}
#endif