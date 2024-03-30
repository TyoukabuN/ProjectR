//-----------------------------------------------------------------------
// <copyright file="ValidationPathStep.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using System.Reflection;

    public struct ValidationPathStep
    {
        public string StepString;
        public object Value;
        public MemberInfo Member;
    }
}
#endif