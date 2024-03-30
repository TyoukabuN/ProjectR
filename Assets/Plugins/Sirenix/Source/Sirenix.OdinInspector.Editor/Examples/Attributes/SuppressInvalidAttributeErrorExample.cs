//-----------------------------------------------------------------------
// <copyright file="SuppressInvalidAttributeErrorExample.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(SuppressInvalidAttributeErrorAttribute))]
    internal class SuppressInvalidAttributeErrorExample
    {
        [Range(0, 10)]
        public string InvalidAttributeError = "This field will have an error box for the Range attribute on a string field.";

        [Range(0, 10), SuppressInvalidAttributeError]
        public string SuppressedError = "The error has been suppressed on this field, and thus no error box will appear.";
    }
}
#endif