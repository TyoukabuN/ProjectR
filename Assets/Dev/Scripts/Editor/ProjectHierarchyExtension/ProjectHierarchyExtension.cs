#if UNITY_EDITOR
using InfinityCode.UltimateEditorEnhancer.ProjectTools;
using PJR.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR
{
    [InitializeOnLoad]
    public partial class ProjectHierarchyExtension
    {
        static ProjectHierarchyExtension()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;

            ProjectItemDrawer.Register("PROJECT_HIERARCHY_EXTENSION", DrawMenu, 100);
        }

        public const string FolderInfoName = "__info__.json";

        public struct FolderInfo
        {
            public string FolderDescription;
        }

        static void DrawMenu(ProjectItem item)
        {
            if (!item.isFolder)
                return;
            string descFilePath = $"{item.path}/{FolderInfoName}";
            var asset =AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(descFilePath);
            if(asset == null)
                return;
            var folderInfo = JsonUtility.FromJson<FolderInfo>(asset.text);
            var labelRect = GUIUtil.GetLabelRect(item, folderInfo.FolderDescription);
            GUI.Label(labelRect, folderInfo.FolderDescription, Label_ModdleRight_Gray);
        }

        private static GUIStyle m_Label_ModdleRight_Gray;
        public static GUIStyle Label_ModdleRight_Gray
        {
            get
            {
                m_Label_ModdleRight_Gray = new GUIStyle(EditorStyles.label);
                m_Label_ModdleRight_Gray.alignment = TextAnchor.MiddleRight;
                m_Label_ModdleRight_Gray.normal.textColor = Color.gray;
                return m_Label_ModdleRight_Gray;
            }
        }
    }
}
#endif
