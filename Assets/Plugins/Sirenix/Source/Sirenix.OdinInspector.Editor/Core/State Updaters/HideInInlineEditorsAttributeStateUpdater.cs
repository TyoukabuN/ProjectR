//-----------------------------------------------------------------------
// <copyright file="HideInInlineEditorsAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.HideInInlineEditorsAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.Drawers;

    public sealed class HideInInlineEditorsAttributeStateUpdater : AttributeStateUpdater<HideInInlineEditorsAttribute>
    {
        public override void OnStateUpdate()
        {
            this.Property.State.Visible = InlineEditorAttributeDrawer.CurrentInlineEditorDrawDepth <= 0;
        }
    }
}
#endif