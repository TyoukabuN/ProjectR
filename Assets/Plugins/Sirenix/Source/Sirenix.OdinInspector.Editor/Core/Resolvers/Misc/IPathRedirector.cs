//-----------------------------------------------------------------------
// <copyright file="IPathRedirector.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    public interface IPathRedirector
    {
        bool TryGetRedirectedProperty(string childName, out InspectorProperty property);
    }
}
#endif