//-----------------------------------------------------------------------
// <copyright file="ExampleTransform.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    public class ExampleTransform : ScriptableObject
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;
    }
}
#endif