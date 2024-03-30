//-----------------------------------------------------------------------
// <copyright file="TypeSearchInfo.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    using System;

    public struct TypeSearchInfo
    {
        public Type MatchType;
        public Type[] Targets;
        public TargetMatchCategory[] TargetCategories;
        public double Priority;
        public object CustomData;
    }
}
#endif