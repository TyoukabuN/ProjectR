//-----------------------------------------------------------------------
// <copyright file="DisableInPlayModeAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.DisableInPlayModeAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor;
    using UnityEngine;

    public class DisableInPlayModeAttributeStateUpdater : AttributeStateUpdater<DisableInPlayModeAttribute>
    {
        public override void OnStateUpdate()
        {
            this.Property.State.Enabled = !Application.isPlaying;
        }
    }
}
#endif