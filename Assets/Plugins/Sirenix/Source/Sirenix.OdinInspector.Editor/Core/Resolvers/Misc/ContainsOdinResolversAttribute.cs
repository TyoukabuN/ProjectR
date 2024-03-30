//-----------------------------------------------------------------------
// <copyright file="ContainsOdinResolversAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.ContainsOdinResolvers]

namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ContainsOdinResolversAttribute : Attribute
    {
    }
}
#endif