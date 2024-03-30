//-----------------------------------------------------------------------
// <copyright file="AtomContainerAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.AtomContainer]

namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AtomContainerAttribute : Attribute
    {
    }
}
#endif