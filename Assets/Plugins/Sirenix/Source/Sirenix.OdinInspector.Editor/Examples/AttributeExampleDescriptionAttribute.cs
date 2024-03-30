//-----------------------------------------------------------------------
// <copyright file="AttributeExampleDescriptionAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class AttributeExampleDescriptionAttribute : Attribute
    {
        public string Description;

        public AttributeExampleDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
#endif