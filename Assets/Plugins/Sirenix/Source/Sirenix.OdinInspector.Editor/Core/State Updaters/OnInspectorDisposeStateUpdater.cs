//-----------------------------------------------------------------------
// <copyright file="OnInspectorDisposeStateUpdater.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.ActionResolvers;
using System;

[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.OnInspectorDisposeStateUpdater), -10000)]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    public sealed class OnInspectorDisposeStateUpdater : AttributeStateUpdater<OnInspectorDisposeAttribute>, IDisposable
    {
        private ActionResolver action;

        protected override void Initialize()
        {
            this.action = ActionResolver.Get(this.Property, this.Attribute.Action);
            this.ErrorMessage = this.action.ErrorMessage;
        }

        public void Dispose()
        {
            this.action.DoActionForAllSelectionIndices();
        }
    }
}
#endif