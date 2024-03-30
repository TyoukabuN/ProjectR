//-----------------------------------------------------------------------
// <copyright file="DerivedTypeMatcher.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.TypeSearch
{
#pragma warning disable

    using System;

    public class DerivedTypeMatcher : TypeMatcher
    {
        private TypeSearchInfo info;

        public override string Name { get { return "Derived Type Match --> Type : Match[<Target>]"; } }

        public override Type Match(Type[] targets, ref bool stopMatching)
        {
            if (targets.Length != this.info.Targets.Length) return null;

            for (int i = 0; i < targets.Length; i++)
            {
                if (typeof(Attribute).IsAssignableFrom(targets[i]) || !this.info.Targets[i].IsAssignableFrom(targets[i])) return null;
            }

            return this.info.MatchType;
        }

        public class Creator : TypeMatcherCreator
        {
            public override bool TryCreateMatcher(TypeSearchInfo info, out TypeMatcher matcher)
            {
                matcher = null;

                if (info.MatchType.IsGenericTypeDefinition || info.MatchType.IsDefined(typeof(DisableNonGenericPolymorphicTypeMatchingAttribute), false)) return false;

                matcher = new DerivedTypeMatcher()
                {
                    info = info
                };

                return true;
            }
        }
    }
}
#endif