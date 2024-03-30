//-----------------------------------------------------------------------
// <copyright file="IMemberSelector.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IMemberSelector
    {
        IList<MemberInfo> SelectMembers(Type type);
    }
}
#endif