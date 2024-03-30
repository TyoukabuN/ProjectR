//-----------------------------------------------------------------------
// <copyright file="EmptyPropertyResolver.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using Sirenix.Utilities.Editor;
    using System;

    [OdinDontRegister]
    public class EmptyPropertyResolver : OdinPropertyResolver
    {
        public override int ChildNameToIndex(string name)
        {
            return -1;
        }

        public override int ChildNameToIndex(ref StringSlice name)
        {
            return -1;
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            throw new NotSupportedException();
        }

        protected override int CalculateChildCount()
        {
            return 0;
        }

        public override bool CanResolveForPropertyFilter(InspectorProperty property)
        {
            return property != property.Tree.RootProperty;
        }
    }
}
#endif