//-----------------------------------------------------------------------
// <copyright file="EnableGUIAttribute.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;

    /// <summary>
    /// <para>An attribute that enables GUI.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// public class InlineEditorExamples : MonoBehaviour
    /// {
    ///     [EnableGUI]
    ///     public string SomeReadonlyProperty { get { return "My GUI is usually disabled." } }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ReadOnlyAttribute"/>
    [AttributeUsage(AttributeTargets.All)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class EnableGUIAttribute : Attribute
    {
    }
}