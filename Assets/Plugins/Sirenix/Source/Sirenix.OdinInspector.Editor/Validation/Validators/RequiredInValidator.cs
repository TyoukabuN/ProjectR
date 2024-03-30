//-----------------------------------------------------------------------
// <copyright file="RequiredInValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.RequiredInValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.ValueResolvers;
    using UnityEngine;

    public class RequiredInValidator<T> : AttributeValidator<RequiredInAttribute, T>
        where T : class
    {
        private ValueResolver<string> errorMessageGetter;
        private bool canValidate;

        protected override void Initialize()
        {
            this.canValidate = (OdinPrefabUtility.GetPrefabKind(this.Property) & this.Attribute.PrefabKind) != 0;
            if (this.Attribute.ErrorMessage != null)
            {
                this.errorMessageGetter = ValueResolver.GetForString(this.Property, this.Attribute.ErrorMessage);
            }
        }

        protected override void Validate(ValidationResult result)
        {
            if (canValidate && !this.IsValid(this.ValueEntry.SmartValue))
            {
                var msg = this.errorMessageGetter != null ? this.errorMessageGetter.GetValue() : (this.Property.NiceName + " is required");
                result.AddError(msg)
                    .WithFix<FixArgs<T>>(x => this.Property.ValueEntry.WeakSmartValue = x.NewValue);
            }
        }

        private bool IsValid(T memberValue)
        {
            if (object.ReferenceEquals(memberValue, null))
                return false;

            if (memberValue is string && string.IsNullOrEmpty(memberValue as string))
                return false;

            if (memberValue is UnityEngine.Object && (memberValue as UnityEngine.Object) == null)
                return false;

            return true;
        }
    }
}
#endif