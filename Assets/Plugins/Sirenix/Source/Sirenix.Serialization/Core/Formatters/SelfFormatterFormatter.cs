//-----------------------------------------------------------------------
// <copyright file="SelfFormatterFormatter.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Sirenix.Serialization
{
#pragma warning disable

    /// <summary>
    /// Formatter for types that implement the <see cref="ISelfFormatter"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="BaseFormatter{T}" />
    public sealed class SelfFormatterFormatter<T> : BaseFormatter<T> where T : ISelfFormatter
    {
        /// <summary>
        /// Calls <see cref="ISelfFormatter.Deserialize" />  on the value to deserialize.
        /// </summary>
        protected override void DeserializeImplementation(ref T value, IDataReader reader)
        {
            value.Deserialize(reader);
        }

        /// <summary>
        /// Calls <see cref="ISelfFormatter.Serialize" />  on the value to deserialize.
        /// </summary>
        protected override void SerializeImplementation(ref T value, IDataWriter writer)
        {
            value.Serialize(writer);
        }
    }

    public sealed class WeakSelfFormatterFormatter : WeakBaseFormatter
    {
        public WeakSelfFormatterFormatter(Type serializedType) : base(serializedType)
        {
        }

        /// <summary>
        /// Calls <see cref="ISelfFormatter.Deserialize" />  on the value to deserialize.
        /// </summary>
        protected override void DeserializeImplementation(ref object value, IDataReader reader)
        {
            ((ISelfFormatter)value).Deserialize(reader);
        }

        /// <summary>
        /// Calls <see cref="ISelfFormatter.Serialize" />  on the value to deserialize.
        /// </summary>
        protected override void SerializeImplementation(ref object value, IDataWriter writer)
        {
            ((ISelfFormatter)value).Serialize(writer);
        }
    }
}