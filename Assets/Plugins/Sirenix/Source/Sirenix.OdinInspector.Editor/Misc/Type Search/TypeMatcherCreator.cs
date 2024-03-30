//-----------------------------------------------------------------------
// <copyright file="TypeMatcherCreator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    public abstract class TypeMatcherCreator
    {
        public abstract bool TryCreateMatcher(TypeSearchInfo info, out TypeMatcher matcher);
    }
}
#endif