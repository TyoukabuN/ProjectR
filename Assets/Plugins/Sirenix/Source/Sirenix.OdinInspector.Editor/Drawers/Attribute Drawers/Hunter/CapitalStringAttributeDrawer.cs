using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor.Drawers
{
    [DrawerPriority(0.3)]
    public class CapitalStringAttributeDrawer : OdinAttributeDrawer<CapitalStringAttribute,string>
    {
        private static HashSet<char> invalidChars = null;
        protected override void DrawPropertyLayout(GUIContent label)
        {
            invalidChars ??= new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());
            EditorGUI.BeginChangeCheck();
            this.CallNextDrawer(label);
            if (EditorGUI.EndChangeCheck())
            {
                var value = this.Property.ValueEntry.WeakSmartValue as string;
                var newStr = "";
                foreach (var c in value)
                {
                    if (invalidChars.Contains(c))
                    {
                        continue;
                    }
                    newStr += c;
                }
                newStr = newStr.ToUpper();
                this.Property.ValueEntry.WeakSmartValue = newStr;
            }
        }
    }
}
