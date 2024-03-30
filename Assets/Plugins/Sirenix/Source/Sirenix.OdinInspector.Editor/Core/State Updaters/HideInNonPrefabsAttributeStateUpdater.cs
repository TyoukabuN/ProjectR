//-----------------------------------------------------------------------
// <copyright file="HideInNonPrefabsAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.HideInNonPrefabsAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    using UnityEditor;

#pragma warning disable CS0618 // Type or member is obsolete
    public sealed class HideInNonPrefabsAttributeStateUpdater : AttributeStateUpdater<HideInNonPrefabsAttribute>
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private bool hide;

        protected override void Initialize()
        {
            var kind = OdinPrefabUtility.GetPrefabKind(this.Property);
            this.hide = kind == PrefabKind.None || (kind & PrefabKind.NonPrefabInstance) != 0;
        }

        public override void OnStateUpdate()
        {
            this.Property.State.Visible = !this.hide;
        }
    }
}
#endif