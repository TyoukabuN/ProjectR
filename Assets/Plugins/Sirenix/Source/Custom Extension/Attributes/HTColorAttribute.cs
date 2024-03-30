namespace Sirenix.OdinInspector
{
#pragma warning disable

    using System;
    using UnityEngine;

    /// </code>
    /// </example>
    /// <seealso cref="ButtonAttribute"/>
    /// <seealso cref="LabelTextAttribute"/>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class HTColorAttribute : Attribute
    {
        public Color Color;

        /// <summary>
        /// Creates a title above any property in the inspector.
        /// </summary>
        /// <param name="title">The title displayed above the property in the inspector.</param>
        /// <param name="subtitle">Optional subtitle</param>
        /// <param name="titleAlignment">Title alignment</param>
        /// <param name="horizontalLine">Horizontal line</param>
        /// <param name="bold">If <c>true</c> the title will be drawn with a bold font.</param>
        public HTColorAttribute(float r = 0, float g = 0, float b = 0)
        {
            this.Color = new Color(r, g, b);
        }
    }
}
