//-----------------------------------------------------------------------
// <copyright file="ShowPropertiesInTheInspectorExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(ShowInInspectorAttribute), Name = "Inspect Properties")]
    internal class ShowPropertiesInTheInspectorExamples
    {
        [SerializeField, HideInInspector]
        private int evenNumber;

        [ShowInInspector]
        public int EvenNumber
        {
            get { return this.evenNumber; }
            set { this.evenNumber = value - (value % 2); }
        }
    }
}
#endif