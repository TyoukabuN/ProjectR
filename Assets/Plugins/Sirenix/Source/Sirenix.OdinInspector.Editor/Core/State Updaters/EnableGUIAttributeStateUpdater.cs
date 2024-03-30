//-----------------------------------------------------------------------
// <copyright file="EnableGUIAttributeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.EnableGUIAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable


    public sealed class EnableGUIAttributeStateUpdater : AttributeStateUpdater<EnableGUIAttribute>
    {
        public override void OnStateUpdate()
        {
            this.Property.State.Enabled = true;
        }
    }
}
#endif