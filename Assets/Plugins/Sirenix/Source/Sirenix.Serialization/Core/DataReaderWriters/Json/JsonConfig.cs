//-----------------------------------------------------------------------
// <copyright file="JsonConfig.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
#pragma warning disable

    /// <summary>
    /// Contains various string constants used by the <see cref="JsonDataWriter"/>, <see cref="JsonDataReader"/> and <see cref="JsonTextReader"/> classes.
    /// </summary>
    public static class JsonConfig
    {
        /// <summary>
        /// The named of a node id entry.
        /// </summary>
        public const string ID_SIG = "$id";

        /// <summary>
        /// The name of a type entry.
        /// </summary>
        public const string TYPE_SIG = "$type";

        /// <summary>
        /// The name of a regular array length entry.
        /// </summary>
        public const string REGULAR_ARRAY_LENGTH_SIG = "$rlength";

        /// <summary>
        /// The name of a primitive array length entry.
        /// </summary>
        public const string PRIMITIVE_ARRAY_LENGTH_SIG = "$plength";

        /// <summary>
        /// The name of a regular array content entry.
        /// </summary>
        public const string REGULAR_ARRAY_CONTENT_SIG = "$rcontent";

        /// <summary>
        /// The name of a primitive array content entry.
        /// </summary>
        public const string PRIMITIVE_ARRAY_CONTENT_SIG = "$pcontent";

        /// <summary>
        /// The beginning of the content of an internal reference entry.
        /// </summary>
        public const string INTERNAL_REF_SIG = "$iref";

        /// <summary>
        /// The beginning of the content of an external reference by index entry.
        /// </summary>
        public const string EXTERNAL_INDEX_REF_SIG = "$eref";

        /// <summary>
        /// The beginning of the content of an external reference by guid entry.
        /// </summary>
        public const string EXTERNAL_GUID_REF_SIG = "$guidref";

        /// <summary>
        /// The beginning of the content of an external reference by string entry. This is an old entry using an invalid data format where the ref string is dumped inline without escaping.
        /// </summary>
        public const string EXTERNAL_STRING_REF_SIG_OLD = "$strref";

        /// <summary>
        /// The beginning of the content of an external reference by string entry. This is a new entry using the valid format where the ref string is written as an escaped string.
        /// </summary>
        public const string EXTERNAL_STRING_REF_SIG_FIXED = "$fstrref";
    }
}