//-----------------------------------------------------------------------
// <copyright file="DelayedPropertyExample.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(DelayedAttribute))]
    [AttributeExample(typeof(DelayedPropertyAttribute))]
    internal class DelayedPropertyExample
    {
        // Delayed and DelayedProperty attributes are virtually identical...
        [Delayed]
        [OnValueChanged("OnValueChanged")]
        public int DelayedField;

        // ... but the DelayedProperty can, as the name suggests, also be applied to properties.
        [ShowInInspector, DelayedProperty]
        [OnValueChanged("OnValueChanged")]
        public string DelayedProperty { get; set; }

        private void OnValueChanged()
        {
            Debug.Log("Value changed!");
        }
    }
}
#endif