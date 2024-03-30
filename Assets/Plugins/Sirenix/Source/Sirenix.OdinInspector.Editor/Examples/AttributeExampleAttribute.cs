//-----------------------------------------------------------------------
// <copyright file="AttributeExampleAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class AttributeExampleAttribute : Attribute
    {
        public Type AttributeType;
        public string Name;
        public string Description;
        public float Order;

        public AttributeExampleAttribute(Type type)
        {
            this.AttributeType = type;
        }

        public AttributeExampleAttribute(Type type, string description)
        {
            this.AttributeType = type;
            this.Description = description;
        }
    }
}
#endif