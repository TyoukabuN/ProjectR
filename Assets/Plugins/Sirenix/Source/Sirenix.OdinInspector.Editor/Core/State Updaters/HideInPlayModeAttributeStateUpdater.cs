//-----------------------------------------------------------------------
// <copyright file="HideInPlayModeAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.HideInPlayModeAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    using UnityEngine;

    public sealed class HideInPlayModeAttributeStateUpdater : AttributeStateUpdater<HideInPlayModeAttribute>
    {
        public override void OnStateUpdate()
        {
            this.Property.State.Visible = !Application.isPlaying;
        }
    }
}
#endif