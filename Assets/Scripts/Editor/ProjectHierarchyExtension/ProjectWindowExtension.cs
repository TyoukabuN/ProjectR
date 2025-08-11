#if UNITY_EDITOR
using System;
using InfinityCode.UltimateEditorEnhancer;
using InfinityCode.UltimateEditorEnhancer.Integration;
using InfinityCode.UltimateEditorEnhancer.ProjectTools;
using Newtonsoft.Json;
using PJR.Timeline;
using PJR.Timeline.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PJR.Editor
{
    [InitializeOnLoad]
    public static class ProjectWindowExtension
    {
        static ProjectWindowExtension()
        {
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
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(descFilePath);
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
    
    [InitializeOnLoad]
    public static class SequenceDirectorButton
    {
        private static Texture2D offTexture;

        static SequenceDirectorButton()
        {
            HierarchyItemDrawer.Register("SequenceDirectorButton", OnHierarchyItem, 100);
        }

        public static bool Contain(GameObject gameObject)
        {
            var director = gameObject.GetComponent<SequenceDirector>();
            return director?.Sequence != null;
        }
        private static void OnHierarchyItem(HierarchyItem item)
        {
            if (item.gameObject == null) return;

            Event e = Event.current;

            bool contain = Contain(item.gameObject);
            if (!contain)
                return;

            Rect rect = item.rect;
            Rect r = new Rect(rect.xMax - 16, rect.y, 16, rect.height);
            if (Cinemachine.ContainBrain(item.gameObject)) r.x -= 16;

            string tooltip = "Open PJR.TimelineWindow";
            GUIContent content = TempContent.Get(DirectorIcon.image, tooltip);
            
            //右键
            if (e.type == EventType.MouseUp && e.button == 1 && r.Contains(e.mousePosition))
            {
                e.Use();
            }

            bool onFocus = TimelineWindow.Selection_IsObjectFocused(item.gameObject);

            if (onFocus)
            {
                GUIHelper.PushColor(Color.green, true);
            }
            if (GUI.Button(r, content, GUIStyle.none))
            {
                TimelineWindow.ShowWindow()?.Selection_TrySelectObject(item.gameObject);
            }

            if (onFocus)
            {
                GUIHelper.PopColor();
            }
            item.rect.xMax -= 16;
        }
        
        
        private static GUIContent _directorIcon;
        public static GUIContent DirectorIcon
        {
            get
            {
                if (_directorIcon == null) _directorIcon = EditorGUIUtility.IconContent("d_PlayableDirector Icon");
                return _directorIcon;
            }
        }
    }
}
#endif
