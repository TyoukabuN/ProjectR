//-----------------------------------------------------------------------
// <copyright file="HideIfGroupAttributeDrawer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
#pragma warning disable

    using UnityEngine;

    public class HideIfGroupAttributeDrawer : OdinGroupDrawer<HideIfGroupAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            for (int i = 0; i < this.Property.Children.Count; i++)
            {
                this.Property.Children[i].Draw();
            }
        }
    }
}
#endif