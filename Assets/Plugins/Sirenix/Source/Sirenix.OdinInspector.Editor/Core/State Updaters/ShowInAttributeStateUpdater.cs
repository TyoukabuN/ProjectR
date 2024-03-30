//-----------------------------------------------------------------------
// <copyright file="ShowInAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.ShowInAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    public sealed class ShowInAttributeStateUpdater : AttributeStateUpdater<ShowInAttribute>
    {
        private bool show;

        protected override void Initialize()
        {
            this.show = (OdinPrefabUtility.GetPrefabKind(this.Property) & this.Attribute.PrefabKind) != 0;
        }

        public override void OnStateUpdate()
        {
            this.Property.State.Visible = this.show;
        }
    }
}
#endif