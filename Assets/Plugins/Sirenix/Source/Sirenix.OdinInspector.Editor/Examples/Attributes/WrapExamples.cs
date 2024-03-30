//-----------------------------------------------------------------------
// <copyright file="WrapExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(WrapAttribute))]
    internal class WrapExamples
    {
        [Wrap(0f, 100f)]
        public int IntWrapFrom0To100;
        
        [Wrap(0f, 100f)]
        public float FloatWrapFrom0To100;
        
        [Wrap(0f, 100f)]
        public Vector3 Vector3WrapFrom0To100;

        [Wrap(0f, 360)]
        public float AngleWrap;

        [Wrap(0f, Mathf.PI * 2)]
        public float RadianWrap;
    }
}
#endif