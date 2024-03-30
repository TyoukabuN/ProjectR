//-----------------------------------------------------------------------
// <copyright file="ValidationEvents.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using System;

    public static class ValidationEvents
    {
        public static event Action<ValidationStateChangeInfo> OnValidationStateChanged;

        internal static void InvokeOnValidationStateChanged(ValidationStateChangeInfo info)
        {
            if (OnValidationStateChanged != null)
            {
                OnValidationStateChanged(info);
            }
        }
    }

    public struct ValidationStateChangeInfo
    {
        public IValidator Validator { get { return this.ValidationResult.Setup.Validator; } }
        public ValidationResult ValidationResult;
    }
}
#endif