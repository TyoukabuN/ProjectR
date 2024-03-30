//-----------------------------------------------------------------------
// <copyright file="ValidatorExtensions.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using Sirenix.Utilities;
using UnityEditor;

namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using Sirenix.OdinInspector;

    public static class ValidatorExtensions
    {
        public static ValidationResultType ToValidationResultType(this InfoMessageType messageType)
        {
            if (messageType == InfoMessageType.Error)
                return ValidationResultType.Error;
            else if (messageType == InfoMessageType.Warning)
                return ValidationResultType.Warning;
            else return ValidationResultType.Valid;
        }
        
        internal static string GetNiceValidatorTypeName(this Type t)
        {
            // Yeah I know, don't judge...

            var k = "";

            k = t.GetNiceName();
            var i = k.IndexOf('<');

            if (i >= 0)
            {
                k = k.Substring(0, i);
            }

            for (int j = 0; j < 2; j++)
            {
                if (k.FastEndsWith("Validator")) k = k.Substring(0, k.Length - "Validator".Length);
                if (k.FastEndsWith("Attribute")) k = k.Substring(0, k.Length - "Attribute".Length);
            }

            return ObjectNames.NicifyVariableName(k);
        }
    }
}
#endif