//-----------------------------------------------------------------------
// <copyright file="DisableInInlineEditorsAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Drawers;

[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.DisableInInlineEditorsAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    public sealed class DisableInInlineEditorsAttributeStateUpdater : AttributeStateUpdater<DisableInInlineEditorsAttribute>
    {
        public override void OnStateUpdate()
        {
            // Only disable, never enable
            if (this.Property.State.Enabled && InlineEditorAttributeDrawer.CurrentInlineEditorDrawDepth > 0)
            {
                this.Property.State.Enabled = false;
            }
        }
    }
}
#endif