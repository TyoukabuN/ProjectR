//-----------------------------------------------------------------------
// <copyright file="HideInEditorModeAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.HideInEditorModeAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    using UnityEngine;

    public sealed class HideInEditorModeAttributeStateUpdater : AttributeStateUpdater<HideInEditorModeAttribute>
    {
        public override void OnStateUpdate()
        {
            this.Property.State.Visible = Application.isPlaying;
        }
    }
}
#endif