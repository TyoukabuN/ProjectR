//-----------------------------------------------------------------------
// <copyright file="NamedValueGetter.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.ActionResolvers
{
#pragma warning disable

    public delegate object NamedValueGetter(ref ActionResolverContext context, int selectionIndex);
}
#endif