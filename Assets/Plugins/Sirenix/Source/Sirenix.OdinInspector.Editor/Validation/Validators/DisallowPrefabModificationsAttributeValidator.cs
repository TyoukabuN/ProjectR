//-----------------------------------------------------------------------
// <copyright file="DisallowModificationsInAttributeValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.DisallowModificationsInAttributeValidator))]

namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using Sirenix.OdinInspector;

    public class DisallowModificationsInAttributeValidator : AttributeValidator<DisallowModificationsInAttribute>
    {
        protected override void Validate(ValidationResult result)
        {
            var kind = OdinPrefabUtility.GetPrefabKind(this.Property);

            if ((kind & this.Attribute.PrefabKind) != 0 && this.Property.ValueEntry.ValueChangedFromPrefab)
            {
                result.AddError($"Modifications on '{this.Attribute.PrefabKind}' for {this.Property.NiceName} are not allowed.");
            }
        }
    }
}
#endif