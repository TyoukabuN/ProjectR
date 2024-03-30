//-----------------------------------------------------------------------
// <copyright file="OdinSerializeAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
#pragma warning disable

    using System;

    /// <summary>
    /// Indicates that an instance field or auto-property should be serialized by Odin.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OdinSerializeAttribute : Attribute
    {
    }

    #region Modified By Hunter (jb) -- 2022年8月22日

    /// <summary>
    /// 使Odin序列化一个类
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct| AttributeTargets.Class, Inherited = true)]
    public class OdinSerializeTypeAttribute : Attribute
    {
    }

    /// <summary>
    /// 使Odin忽略一个字段的序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OdinSerializeIgnoreAttribute : Attribute
    {
    }
    
    /// <summary>
    /// 使Odin忽略一个字段的序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class OdinInitOnDeserialization : Attribute
    {
    }
    #endregion
}