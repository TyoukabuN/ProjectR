//-----------------------------------------------------------------------
// <copyright file="TypeFormatterLocator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Sirenix.Serialization;

[assembly: RegisterFormatterLocator(typeof(TypeFormatterLocator), -70)]

namespace Sirenix.Serialization
{
#pragma warning disable

    using System;

    internal class TypeFormatterLocator : IFormatterLocator
    {
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, bool allowWeakFallbackFormatters, out IFormatter formatter)
        {
            if (!typeof(Type).IsAssignableFrom(type))
            {
                formatter = null;
                return false;
            }

            formatter = new TypeFormatter();
            return true;
        }
    }
}