//-----------------------------------------------------------------------
// <copyright file="StaticInitializeBeforeDrawingAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class StaticInitializeBeforeDrawingAttribute : Attribute
    {
        public StaticInitializeBeforeDrawingAttribute(params Type[] types)
        {
            this.Types = types;
        }

        public Type[] Types;
    }
}
#endif