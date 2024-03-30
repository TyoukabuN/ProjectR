//-----------------------------------------------------------------------
// <copyright file="ProjectSettingKeyAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Internal
{
#pragma warning disable

    using System;

    public class ProjectSettingKeyAttribute : Attribute
    {
        public readonly string Key;
        public readonly object DefaultValue;

        public ProjectSettingKeyAttribute(string key, object defaultValue)
        {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
#endif