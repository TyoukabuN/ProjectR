//-----------------------------------------------------------------------
// <copyright file="DrawerChainResolver.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    public abstract class DrawerChainResolver
    {
        public abstract DrawerChain GetDrawerChain(InspectorProperty property);
    }
}
#endif