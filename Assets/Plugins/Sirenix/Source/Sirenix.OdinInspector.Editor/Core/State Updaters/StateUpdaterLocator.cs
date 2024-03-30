//-----------------------------------------------------------------------
// <copyright file="StateUpdaterLocator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    public abstract class StateUpdaterLocator
    {
        public abstract StateUpdater[] GetStateUpdaters(InspectorProperty property);
    }
}
#endif