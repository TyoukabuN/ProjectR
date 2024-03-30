//-----------------------------------------------------------------------
// <copyright file="SerializationPolicyMemberSelector.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using Sirenix.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class SerializationPolicyMemberSelector : IMemberSelector
    {
        public readonly ISerializationPolicy Policy;

        public SerializationPolicyMemberSelector(ISerializationPolicy policy)
        {
            this.Policy = policy;
        }

        public IList<MemberInfo> SelectMembers(Type type)
        {
            return FormatterUtilities.GetSerializableMembers(type, this.Policy);
        }
    }
}
#endif