//-----------------------------------------------------------------------
// <copyright file="EmittedFormatterAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
#pragma warning disable

    using System;

    /// <summary>
    /// Indicates that this formatter type has been emitted. Never put this on a type!
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EmittedFormatterAttribute : Attribute
    {
    }
}