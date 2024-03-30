//-----------------------------------------------------------------------
// <copyright file="TypeSearchResult.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    using System;

    public struct TypeSearchResult
    {
        public TypeSearchInfo MatchedInfo;
        public Type MatchedType;
        public Type[] MatchedTargets;
        public TypeMatcher MatchedMatcher;
        public TypeMatchRule MatchedRule;
        public TypeSearchIndex MatchedIndex;
    }
}
#endif