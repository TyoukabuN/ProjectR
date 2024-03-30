//-----------------------------------------------------------------------
// <copyright file="EnumSerializer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
#pragma warning disable
    
    using System;
    //说明:https://lswiki.hunterstudio.cn/en/%E5%BC%80%E5%8F%91/Odin%E5%BA%8F%E5%88%97%E5%8C%96%E5%A4%84%E7%90%86/%E6%9E%9A%E4%B8%BE%E5%BA%8F%E5%88%97%E5%8C%96%E5%AD%97%E7%AC%A6%E5%A4%87%E4%BB%BD
    public class EnumSerializeStringBackUp : System.Attribute
    {
    }
    /// <summary>
    /// Serializer for all enums.
    /// </summary>
    /// <typeparam name="T">The type of the enum to serialize and deserialize.</typeparam>
    /// <seealso cref="Serializer{T}" />
#if CSHARP_7_3_OR_NEWER
    public unsafe sealed class EnumSerializer<T> : Serializer<T> where T : unmanaged, Enum
    {
        private static readonly int SizeOf_T = sizeof(T);
        
        #region Modified By Hunter (JB) -- 2022年3月2日
        static EnumSerializer()
        {
            var attrs = typeof(T).GetCustomAttributes(typeof(EnumSerializeStringBackUp), false);
            StringOverrrideInt = attrs.Length > 0 && attrs[0] != null;
        }
        #endregion
#else
    public sealed class EnumSerializer<T> : Serializer<T>
    {
        
        static EnumSerializer()
        {
            if (typeof(T).IsEnum == false)
            {
                throw new Exception("Type " + typeof(T).Name + " is not an enum.");
            }
            #region Modified By Hunter (jb) -- 2022年3月2日 枚举序列化字符覆盖
            StringOverrrideInt = attrs.Length > 0 && attrs[0] != null; 
            #endregion
        }
#endif
        #region Modified By Hunter (jb) -- 2022年3月2日 

        private static readonly bool StringOverrrideInt;
        private const string STRING_BACKUP_NODE_NAME = "enumStrValueBackup";

        #endregion
        
    /// <summary>
        /// Reads an enum value of type <see cref="T" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        /// The value which has been read.
        /// </returns>
        public override T ReadValue(IDataReader reader)
        {
            #region Modified By Hunter (jb) -- 2022年3月2日
            string name;

            T ParseIntEntry(EntryType intEntry)
            {
                if (intEntry == EntryType.Integer)
                {
                    ulong value;
                    if (reader.ReadUInt64(out value) == false)
                    {
                        reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + intEntry.ToString());
                    }

#if CSHARP_7_3_OR_NEWER
                    return *(T*)&value;
#else
                return (T)Enum.ToObject(typeof(T), value);
#endif
                }
                else
                {
                    reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + intEntry.ToString());
                    reader.SkipEntry();
                    return default(T);
                }    
            }

            if (StringOverrrideInt)
            {
                var entry = reader.PeekEntry(out name);
                //从上一次整型解析出来的枚举值
                var intEntryEnumValue = ParseIntEntry(entry);
                var peekEntry = reader.PeekEntry(out name);
                if (peekEntry == EntryType.String)
                {
                    if (string.Equals(name, STRING_BACKUP_NODE_NAME))
                    {
                        if (!reader.ReadString(out var enumStr))
                        {
                            reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + STRING_BACKUP_NODE_NAME + "' of type " + entry.ToString());
                            return intEntryEnumValue;
                        }
                        if (!Enum.TryParse<T>(enumStr, out T strOvrValue))
                        {
                            reader.Context.Config.DebugContext.LogWarning($"解析枚举{typeof(T).ToString()} \"{enumStr}\" 失败'");
                            return intEntryEnumValue;
                        }

                        if (intEntryEnumValue.Equals(strOvrValue))
                        {
                            return intEntryEnumValue;
                        }
                        reader.Context.Config.DebugContext.LogWarning($"枚举字符值[{strOvrValue}]覆写[{intEntryEnumValue}]");
                        return strOvrValue;
                    }
                } 
                return intEntryEnumValue;
            }
            else
            {
                var entry = reader.PeekEntry(out name);
                //从上一次整型解析出来的枚举值
                var intEntryEnumValue = ParseIntEntry(entry);
                var peekEntry = reader.PeekEntry(out name);
                if (peekEntry == EntryType.String && string.Equals(name, STRING_BACKUP_NODE_NAME))
                {
                    reader.SkipEntry();
                }
                return intEntryEnumValue;
            }
            #endregion
             #region Origin
//
//             string name;
//             var entry = reader.PeekEntry(out name);
//
//             void ParseIntEntry(EntryType intEntry)
//             {
//                 
//             }
//             if (entry == EntryType.Integer)
//             {
//                 ulong value;
//                 if (reader.ReadUInt64(out value) == false)
//                 {
//                     reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entry.ToString());
//                 }
//
// #if CSHARP_7_3_OR_NEWER
//                 return *(T*)&value;
// #else
//                 return (T)Enum.ToObject(typeof(T), value);
// #endif
//             }
//             else
//             {
//                 reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entry.ToString());
//                 reader.SkipEntry();
//                 return default(T);
//             }

            #endregion
        }

        /// <summary>
        /// Writes an enum value of type <see cref="T" />.
        /// </summary>
        /// <param name="name">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="writer">The writer to use.</param>
        public override void WriteValue(string name, T value, IDataWriter writer)
        {
            ulong ul;

            FireOnSerializedType();
            
            
#if CSHARP_7_3_OR_NEWER
            byte* toPtr = (byte*)&ul;
            byte* fromPtr = (byte*)&value;

            for (int i = 0; i < SizeOf_T; i++)
            {
                *toPtr++ = *fromPtr++;
            }
#else
            try
            {
                ul = Convert.ToUInt64(value as Enum);
            }
            catch (OverflowException)
            {
                unchecked
                {
                    ul = (ulong)Convert.ToInt64(value as Enum);
                }
            }
#endif
            writer.WriteUInt64(name, ul);
            #region Modified By Hunter (jb) -- 2022年3月2日

            if (StringOverrrideInt)
            {
                writer.WriteString(STRING_BACKUP_NODE_NAME, value.ToString());
            }

            #endregion
            #region Origin
//
// #if CSHARP_7_3_OR_NEWER
//             byte* toPtr = (byte*)&ul;
//             byte* fromPtr = (byte*)&value;
//
//             for (int i = 0; i < SizeOf_T; i++)
//             {
//                 *toPtr++ = *fromPtr++;
//             }
// #else
//             try
//             {
//                 ul = Convert.ToUInt64(value as Enum);
//             }
//             catch (OverflowException)
//             {
//                 unchecked
//                 {
//                     ul = (ulong)Convert.ToInt64(value as Enum);
//                 }
//             }
// #endif
//
//             writer.WriteUInt64(name, ul);

            #endregion
        }
    }
}