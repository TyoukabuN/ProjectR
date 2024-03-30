//-----------------------------------------------------------------------
// <copyright file="ValidationSetup.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    public struct ValidationSetup
    {
        public IValidator Validator;
        [System.Obsolete("This field is no longer populated by the validation system, as it was never used and caused a lot of garbage allocation.",
#if SIRENIX_INTERNAL
            true
#else
            false
#endif
            )]
        public object Value;
        public object ParentInstance;
        public object Root;
    }
}
#endif