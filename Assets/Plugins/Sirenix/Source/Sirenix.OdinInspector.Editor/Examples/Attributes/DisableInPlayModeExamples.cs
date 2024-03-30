//-----------------------------------------------------------------------
// <copyright file="DisableInPlayModeExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using UnityEngine;

    [AttributeExample(typeof(DisableInPlayModeAttribute))]
    internal class DisableInPlayModeExamples
    {
        [Title("Disabled in play mode")]
        [DisableInPlayMode]
        public int A;

        [DisableInPlayMode]
        public Material B;
    }
}
#endif