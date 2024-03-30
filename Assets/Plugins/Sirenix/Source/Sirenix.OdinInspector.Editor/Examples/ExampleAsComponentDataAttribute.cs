//-----------------------------------------------------------------------
// <copyright file="ExampleAsComponentDataAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples.Internal
{
#pragma warning disable

    using System;

    public class ExampleAsComponentDataAttribute : Attribute
    {
        public string[] AttributeDeclarations;
        public string[] Namespaces;
    }
}
#endif