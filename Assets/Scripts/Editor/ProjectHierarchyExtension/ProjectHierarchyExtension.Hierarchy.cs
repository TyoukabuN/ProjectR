#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PJR.ClassExtension;
using PJR.Editor;

namespace PJR
{
    public partial class ProjectHierarchyExtension 
    {
        static Dictionary<string, GUIContent> s_HierarchyDesc = new Dictionary<string, GUIContent>()
        {
            {"SceneRoot",new GUIContent("场景根对象")},
            {"Navigation",new GUIContent("导航寻路相关")},
            {"Background",new GUIContent("远景天空盒等")},
            {"Grounds",new GUIContent("地面")},
            {"Buildings",new GUIContent("建筑")},
            {"Plants",new GUIContent("植物")},
            {"Effects",new GUIContent("特效")},
            {"Lights",new GUIContent("灯光")},
            {"Cameras",new GUIContent("相机")},
            {"Animations",new GUIContent("动画预制")},
            {"Procedure",new GUIContent("程序控制")},
            {"Traps",new GUIContent("陷阱预设")},
        };

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
                return;

            GUIContent content;
            if (s_HierarchyDesc.TryGetValue(go.name, out content))
            {
                EditorGUI.LabelField(selectionRect, content, Styles.GetMiddleRightLabel);
            }
        }

        [MenuItem("GameObject/PJR/复制Hierarchy路径", false)]
        public static void CopyPath()
        {
            var path = TransformExtension.CopyHierarchyPath(Selection.activeGameObject?.transform);
            Debug.Log(path);
            var textEditor = new TextEditor();
            textEditor.text = path;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        [MenuItem("Assets/PJR/复制GUID", false)]
        public static void GetAssetGUID()
        {
            if (Selection.activeObject == null)
                return;
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(assetPath))
                return;
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
                return;
            Debug.Log($"{guid}\n{assetPath}");
            var textEditor = new TextEditor();
            textEditor.text = guid;
            textEditor.OnFocus();
            textEditor.Copy();
        }
    }
}
#endif

