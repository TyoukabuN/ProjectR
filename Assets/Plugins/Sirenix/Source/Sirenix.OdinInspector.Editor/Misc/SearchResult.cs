//-----------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System.Collections.Generic;

    public class SearchResult
    {
        public InspectorProperty MatchedProperty;
        public List<SearchResult> ChildResults = new List<SearchResult>();
    }
}
#endif