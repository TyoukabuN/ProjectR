//-----------------------------------------------------------------------
// <copyright file="DelegateFormatterLocator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Sirenix.Serialization;

[assembly: RegisterFormatterLocator(typeof(DelegateFormatterLocator), -50)]

namespace Sirenix.Serialization
{
#pragma warning disable

    using Sirenix.Serialization.Utilities;
    using System;

    internal class DelegateFormatterLocator : IFormatterLocator
    {
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, bool allowWeakFallbackFormatters, out IFormatter formatter)
        {
            if (!typeof(Delegate).IsAssignableFrom(type))
            {
                formatter = null;
                return false;
            }

            try
            {
                formatter = (IFormatter)Activator.CreateInstance(typeof(DelegateFormatter<>).MakeGenericType(type));
            }
            catch (Exception ex)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (allowWeakFallbackFormatters && (ex is ExecutionEngineException || ex.GetBaseException() is ExecutionEngineException))
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    formatter = new WeakDelegateFormatter(type);
                }
                else throw;
            }

            return true;
        }
    }
}