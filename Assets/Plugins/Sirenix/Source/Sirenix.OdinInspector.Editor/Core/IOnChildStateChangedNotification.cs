//-----------------------------------------------------------------------
// <copyright file="IOnChildStateChangedNotification.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    public interface IOnChildStateChangedNotification
    {
        void OnChildStateChanged(int childIndex, string state);
    }
}
#endif