//-----------------------------------------------------------------------
// <copyright file="DisallowModificationsInAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.DisallowModificationsInAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    public sealed class DisallowModificationsInAttributeStateUpdater : AttributeStateUpdater<DisallowModificationsInAttribute>
    {
        private bool disable;

        protected override void Initialize()
        {
            this.disable = (OdinPrefabUtility.GetPrefabKind(this.Property) & this.Attribute.PrefabKind) != 0;
        }

        public override void OnStateUpdate()
        {
            // Only disable, never enable
            if (this.Property.State.Enabled && this.disable)
            {
                this.Property.State.Enabled = false;
            }
        }
    }
}
#endif