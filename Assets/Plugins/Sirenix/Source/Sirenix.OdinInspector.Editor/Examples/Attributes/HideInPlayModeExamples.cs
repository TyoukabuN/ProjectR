//-----------------------------------------------------------------------
// <copyright file="HideInPlayModeExamples.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    [AttributeExample(typeof(HideInPlayModeAttribute))]
    internal class HideInPlayModeExamples
    {
        [Title("Hidden in play mode")]
        [HideInPlayMode]
        public int A;

        [HideInPlayMode]
        public int B;
    }
}
#endif