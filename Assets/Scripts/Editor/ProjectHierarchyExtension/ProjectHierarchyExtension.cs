#if UNITY_EDITOR
using System;
using System.Reflection;
using InfinityCode.UltimateEditorEnhancer;
using InfinityCode.UltimateEditorEnhancer.ProjectTools;
using Mono.CompilerServices.SymbolWriter;
using PJR.Editor;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace PJR
{
    [InitializeOnLoad]
    public partial class ProjectHierarchyExtension
    {
        static ProjectHierarchyExtension()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;

            ProjectItemDrawer.Register("PROJECT_HIERARCHY_EXTENSION", DrawMenu, 1000);
        }

        public const string FolderInfoName = "__info__.json";

        public struct FolderInfoWithMenu
        {
            public string FolderDescription;
            public MenuItemDescription[] LeftClickMenuDescription;
        }

        public struct MenuItemDescription
        {
            public string MenuName;
            public string ClassName;
            public string MethodName;
        }

        static void DrawMenu(ProjectItem item)
        {
            if (!item.isFolder)
                return;
            string descFilePath = $"{item.path}/{FolderInfoName}";
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(descFilePath);
            if (asset == null)
                return;

            var folderInfo = JsonConvert.DeserializeObject<FolderInfoWithMenu>(asset.text);
            bool anyLeftClickMenuDesc = folderInfo.LeftClickMenuDescription?.Length > 0;

            var labelRect = GUIUtil.GetLabelRect(item, folderInfo.FolderDescription);
            GUI.Label(labelRect, folderInfo.FolderDescription, anyLeftClickMenuDesc ? Label_ModdleRight_Green : Label_ModdleRight_Gray);

            ButtonEvent button = GUILayoutUtils.Button(labelRect, TempContent.Get(string.Empty, String_HadLefClickMenu),
                GUIStyle.none);
            if (button == ButtonEvent.click)
            {
                if (folderInfo.LeftClickMenuDescription != null)
                {
                    var menu = new GenericMenu();
                    foreach (var desc in folderInfo.LeftClickMenuDescription)
                    {
                        menu.AddItem(new GUIContent(desc.MenuName), false, ()=> HandleMenuItemDescription(desc));
                    }

                    menu.ShowAsContext();
                }
            }
        }

        public static void HandleMenuItemDescription(MenuItemDescription desc)
        {
            var type = Type.GetType(desc.ClassName);
            if (type == null)
            {
                Debug.Log($"failed to find type: {desc.ClassName}");
                return;
            }
            var methodInfo = type.GetMethod(desc.MethodName);
            if (methodInfo == null)
            {
                Debug.Log($"failed to find method: {desc.ClassName}.{desc.MethodName}");
                return;
            }

            methodInfo.Invoke(null, null);
        }

        const string String_HadLefClickMenu = "有左键按钮";

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
        private static GUIStyle m_Label_ModdleRight_Green;
        public static GUIStyle Label_ModdleRight_Green
        {
            get
            {
                m_Label_ModdleRight_Green = new GUIStyle(EditorStyles.label);
                m_Label_ModdleRight_Green.alignment = TextAnchor.MiddleRight;
                m_Label_ModdleRight_Green.normal.textColor = m_Label_ModdleRight_Green.hover.textColor;
                m_Label_ModdleRight_Green.hover.textColor = Color.green;
                return m_Label_ModdleRight_Green;
            }
        }
    }
}
#endif
