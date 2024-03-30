//-----------------------------------------------------------------------
// <copyright file="SdfIconTypeDrawer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.Internal;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [DrawerPriority(0, 0, 1)]
    public class SdfIconTypeDrawer : OdinValueDrawer<SdfIconType>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.GetFeatureRichControlRect(label, out int controlId, out bool hasKeyboardFocus, out Rect valueRect);

            var selected = this.ValueEntry.SmartValue;
            var valLabel = GUIHelper.TempContent(selected.ToString(), EditorIcons.Transparent.Active);
            var hover = valueRect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.popup.Draw(valueRect, valLabel, controlId, hover, hasKeyboardFocus);
                SdfIcons.DrawIcon(valueRect.Padding(4, 2).AlignLeft(valueRect.height), selected);
            }

            var clicked = Event.current.type == EventType.MouseDown && hover;
            var returnKey = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;

            if (clicked || returnKey)
            {
                Event.current.Use();
            }

            this.ValueEntry.SmartValue = SdfIconSelector.SelectIcon(this.ValueEntry.SmartValue, controlId, clicked || returnKey);
        }
    }
}
#endif