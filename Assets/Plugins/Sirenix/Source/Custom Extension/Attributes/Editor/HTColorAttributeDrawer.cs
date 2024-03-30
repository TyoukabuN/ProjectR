#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="TitleAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.ValueResolvers;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="TestAttribute"/>.
    /// </summary>
    /// <seealso cref="TestAttribute"/>
    /// <seealso cref="TitleGroupAttribute"/>
    [DrawerPriority(1, 0, 0)]
    public sealed class HTColorAttributeDrawer : OdinAttributeDrawer<HTColorAttribute>
    {
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Event.current.rawType == EventType.Repaint)
            {
                GUIHelper.PushColor(this.Attribute.Color);
            }

            // Don't draw added emtpy space for the first property.
            if (this.Property != this.Property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            SirenixEditorGUI.BeginBox();
            //if (valid)
            //{
            //    SirenixEditorGUI.Title(
            //        this.titleResolver.GetValue(),
            //        this.subtitleResolver.GetValue(),
            //        (TextAlignment)this.Attribute.TitleAlignment,
            //        this.Attribute.HorizontalLine,
            //        this.Attribute.Bold);
            //}
            //for (int i = 0; i < this.Property.Children.Count; i++)
            //{
            //    var child = this.Property.Children[i];
            //    child.Draw(child.Label);
            //}

            this.CallNextDrawer(label);
            SirenixEditorGUI.EndBox();

            if (Event.current.rawType == EventType.Repaint)
            {
                GUIHelper.PopColor();
            }
        }
    }
}
#endif