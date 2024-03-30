//-----------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;
    using UnityEngine;

    public static class ExceptionExtensions
    {
        public static bool IsExitGUIException(this Exception ex)
        {
            do
            {
                if (ex is ExitGUIException) return true;
                ex = ex.InnerException;
            }
            while (ex != null);

            return false;
        }

        public static ExitGUIException AsExitGUIException(this Exception ex)
        {
            do
            {
                if (ex is ExitGUIException) return ex as ExitGUIException;
                ex = ex.InnerException;
            }
            while (ex != null);

            return null;
        }

        /// <summary>
        /// Unwraps TargetInvocationException and TypeInitializationException
        /// </summary>
        public static Exception UnwrapException(this Exception ex)
        {
            while (ex != null && (
                ex is System.Reflection.TargetInvocationException || 
                ex is System.TypeInitializationException
                ))
            {
                ex = ex.InnerException;
            }

            return ex;
        }
    }
}
#endif