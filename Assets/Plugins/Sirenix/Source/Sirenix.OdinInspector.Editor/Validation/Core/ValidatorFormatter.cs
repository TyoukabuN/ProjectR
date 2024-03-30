//-----------------------------------------------------------------------
// <copyright file="ValidatorFormatter.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.Serialization.RegisterFormatter(typeof(Sirenix.OdinValidator.Editor.ValidatorFormatter<>))]

namespace Sirenix.OdinValidator.Editor
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Serialization;

    public class ValidatorFormatter<T> : ReflectionOrEmittedBaseFormatter<T> where T : Validator, new()
    {
        protected override T GetUninitializedObject()
        {
            return new T();
        }
    }
}
#endif