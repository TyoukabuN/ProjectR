//-----------------------------------------------------------------------
// <copyright file="EnableGUIExample.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    [AttributeExample(typeof(EnableGUIAttribute))]
    internal class EnableGUIExample
    {
        [ShowInInspector]
        public int GUIDisabledProperty { get { return 10; } }

        [ShowInInspector, EnableGUI]
        public int GUIEnabledProperty { get { return 10; } }
    }
}
#endif