//-----------------------------------------------------------------------
// <copyright file="EnumPagingExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    [AttributeExample(typeof(EnumPagingAttribute))]
    internal class EnumPagingExamples
    {
        [EnumPaging]
        public SomeEnum SomeEnumField;
        
        public enum SomeEnum
        {
            A, B, C
        }
    }
}
#endif