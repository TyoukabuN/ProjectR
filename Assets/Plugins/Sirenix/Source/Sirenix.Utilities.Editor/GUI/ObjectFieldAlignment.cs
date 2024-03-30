//-----------------------------------------------------------------------
// <copyright file="ObjectFieldAlignment.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.Utilities.Editor
{
#pragma warning disable

    /// <summary>
    /// How the square object field should be aligned.
    /// </summary>
    /// <seealso cref="PreviewFieldAttribute"/>
    public enum ObjectFieldAlignment
    {
        /// <summary>
        /// Left aligned.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Centered.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Right aligned.
        /// </summary>
        Right = 2,
    }
}
#endif