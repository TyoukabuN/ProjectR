//-----------------------------------------------------------------------
// <copyright file="RegisterDefaultActionResolverAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.ActionResolvers
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RegisterDefaultActionResolverAttribute : Attribute
    {
        public Type ResolverType;
        public double Order;

        public RegisterDefaultActionResolverAttribute(Type resolverType, double order)
        {
            this.ResolverType = resolverType;
            this.Order = order;
        }
    }
}
#endif