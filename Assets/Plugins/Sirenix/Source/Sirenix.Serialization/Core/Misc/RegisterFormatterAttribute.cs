//-----------------------------------------------------------------------
// <copyright file="RegisterFormatterAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterFormatterAttribute : Attribute
    {
        public Type FormatterType { get; private set; }
        public Type WeakFallback { get; private set; }
        public int Priority { get; private set; }

        public RegisterFormatterAttribute(Type formatterType, int priority = 0)
        {
            this.FormatterType = formatterType;
            this.Priority = priority;
        }

        public RegisterFormatterAttribute(Type formatterType, Type weakFallback, int priority = 0)
        {
            this.FormatterType = formatterType;
            this.WeakFallback = weakFallback;
            this.Priority = priority;
        }
    }
}