#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PJR
{
    [InitializeOnLoad]
    public partial class ProjectHierarchyExtension : MonoBehaviour
    {
        static ProjectHierarchyExtension()
        {
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }

        static Dictionary<string, string> type2desc_PathTemp = new Dictionary<string, string>();

        static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(path))
                return; 

            if (type2desc_PathTemp != null && type2desc_PathTemp.TryGetValue(guid, out var desc))
            {
                EditorGUI.LabelField(selectionRect, desc, GetLabelStyle());
                return; 
            }

            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (asset == null) 
                return;

            if (type2desc != null && type2desc.TryGetValue(asset.GetType().FullName, out desc))
            {
                EditorGUI.LabelField(selectionRect, desc, GetLabelStyle());
                return;
            }
        }

        static GUIStyle label;
        static GUIStyle label2;
        static GUIStyle GetLabelStyle(bool isHierarchy = false)
        {
            GUIStyle style = null;
            if (!isHierarchy)
            {
                if (label == null)
                {
                    label = new GUIStyle(EditorStyles.label);
                    label.alignment = TextAnchor.MiddleRight;
                    label.padding.right = 10;
                    label.normal.textColor = Color.gray;
                }
                style = label;
            }
            else
            {
                if (label2 == null)
                {
                    label2 = new GUIStyle(EditorStyles.label);
                    label2.alignment = TextAnchor.MiddleRight;
                    label2.padding.right = 10;
                    label2.normal.textColor = Color.gray;
                }
                style = label2;
            }

            return style;
        }
    }
}
#endif
