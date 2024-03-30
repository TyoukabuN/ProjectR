//-----------------------------------------------------------------------
// <copyright file="ResolverFunc.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.ValueResolvers
{
#pragma warning disable

    using System;

    public delegate TResult ValueResolverFunc<TResult>(ref ValueResolverContext context, int selectionIndex);
}
#endif