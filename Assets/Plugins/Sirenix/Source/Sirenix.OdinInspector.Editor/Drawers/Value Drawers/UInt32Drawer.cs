//-----------------------------------------------------------------------
// <copyright file="UInt32Drawer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
#pragma warning disable

    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Uint property drawer.
    /// </summary>
    public sealed class UInt32Drawer : OdinValueDrawer<uint>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            long value = SirenixEditorFields.LongField(label, entry.SmartValue);

            if (value > uint.MaxValue)
            {
                value = uint.MaxValue;
            }
            else if (value < 0)
            {
                value = 0;
            }

            entry.SmartValue = (uint)value;
        }
    }
}
#endif