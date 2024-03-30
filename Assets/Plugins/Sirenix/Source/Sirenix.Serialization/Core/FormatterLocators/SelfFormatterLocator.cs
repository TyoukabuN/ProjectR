//-----------------------------------------------------------------------
// <copyright file="SelfFormatterLocator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Sirenix.Serialization;

[assembly: RegisterFormatterLocator(typeof(SelfFormatterLocator), -60)]

namespace Sirenix.Serialization
{
#pragma warning disable

    using System;
    using Utilities;

    internal class SelfFormatterLocator : IFormatterLocator
    {
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, bool allowWeakFallbackFormatters, out IFormatter formatter)
        {
            formatter = null;

            if (!typeof(ISelfFormatter).IsAssignableFrom(type)) return false;

            if ((step == FormatterLocationStep.BeforeRegisteredFormatters && type.IsDefined<AlwaysFormatsSelfAttribute>())
                || step == FormatterLocationStep.AfterRegisteredFormatters)
            {
                try
                {
                    formatter = (IFormatter)Activator.CreateInstance(typeof(SelfFormatterFormatter<>).MakeGenericType(type));
                }
                catch (Exception ex)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    if (allowWeakFallbackFormatters && (ex is ExecutionEngineException || ex.GetBaseException() is ExecutionEngineException))
#pragma warning restore CS0618 // Type or member is obsolete
                    {
                        formatter = new WeakSelfFormatterFormatter(type);
                    }
                    else throw;
                }

                return true;
            }

            return false;
        }
    }
}