#if UNITY_EDITOR
using System;
using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJR
{
    public class ResourceDescriptionWindow : OdinEditorWindow
    {
        [MenuItem(@"YooAsset/AssetBundle ResourceDescription")]
        static void Open()
        {
            handle = GetWindow<ResourceDescriptionWindow>("资源描述");
            handle?.LoadDesc();
        }

        public static ResourceDescriptionWindow handle;
        public static ResourceDescription resourceDesc;

        public static PropertyTree resourceDescPropTree;


        static Vector2 scroll_value = Vector2.zero;

        static string buildCommitPathAddedDesc = string.Empty;

        protected override void OnEnable()
        {
            LoadDesc();
        }

        protected override void OnImGUI()
        {
            scroll_value = EditorGUILayout.BeginScrollView(scroll_value);

            DrawResourceDescription();
            GUILayout.FlexibleSpace();

            //Tools
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DrawUpdatePathTool();
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 资源描述
        /// </summary>
        void DrawResourceDescription()
        {
            resourceDescPropTree = resourceDescPropTree ?? PropertyTree.Create(resourceDesc);
            resourceDescPropTree.Draw(false);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save"))
                    SaveDesc();
                if (GUILayout.Button("Load"))
                    LoadDesc();
                if (GUILayout.Button("定位描述文件"))
                    PingDescFile();
                GUILayout.EndHorizontal();
            }
        }

        static AnimBool animBool_tool;
        /// <summary>
        /// 为资源上传目录添加描述
        /// </summary>
        void DrawUpdatePathTool()
        {
            animBool_tool = animBool_tool ?? new AnimBool(true);
            animBool_tool.target = Foldout(animBool_tool.target, "工具");
            if (EditorGUILayout.BeginFadeGroup(animBool_tool.faded))
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("资源上传目录", GUILayout.Width(150f));
                    var buildCommitPath = EditorGUILayout.TextField(GetBuildCommitPath());
                    SetBuildCommitPath(buildCommitPath);
                    if (GUILayout.Button("添加描述"))
                        buildCommitPathAddedDesc = FormatPath(buildCommitPath, resourceDesc.Channel, resourceDesc.Platform);
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.TextField(buildCommitPathAddedDesc);
                    if (GUILayout.Button("复制")) CopyToClipboard(buildCommitPathAddedDesc);
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        string GetDescFilePath()
        {
            return $"{Application.streamingAssetsPath}/{nameof(ResourceDescription)}";
        }
        string GetDescFileRelativePath()
        {
            return $"Assets/StreamingAssets/{nameof(ResourceDescription)}";
        }
        void PingDescFile()
        {
            var descFile = AssetDatabase.LoadAssetAtPath<Object>(GetDescFileRelativePath());
            if (descFile != null)
                EditorGUIUtility.PingObject(descFile);
        }
        void SaveDesc()
        {
            if (resourceDesc == null)
                return;

            string filePath = GetDescFilePath();
            string json = JsonUtility.ToJson(resourceDesc, true);
            File.WriteAllText(filePath, json);
        }
        void LoadDesc()
        {
            resourceDesc = null;
            string filePath = GetDescFilePath();
            if (!File.Exists(filePath))
            {
                resourceDesc = new ResourceDescription();
                SaveDesc();
                return;
            }
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    try
                    {
                        resourceDesc = JsonUtility.FromJson<ResourceDescription>(reader.ReadToEnd());
                        CreateDescPropertyTree(resourceDesc);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        resourceDesc = new ResourceDescription();
                    }
                }
            }
        }
        void CreateDescPropertyTree(ResourceDescription desc)
        {
            if (resourceDescPropTree != null)
                resourceDescPropTree.Dispose();
            resourceDescPropTree = PropertyTree.Create(resourceDesc);
        }
        public static string RegularPath(string path)
        {
            return path.Replace('\\', '/').Replace("\\", "/");
        }

        public void CopyToClipboard(string str)
        {
            TextEditor textEditor = new TextEditor();
            textEditor.text = str;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        string FormatPath(string server, string channel, string platform)
        {
            server = server.TrimEnd('/', ' ', '\\');
            string res;
            if (!string.IsNullOrEmpty(channel) && !string.IsNullOrEmpty(platform))
                res = $"{server}/{channel}/{platform}";
            else if (!string.IsNullOrEmpty(channel))
                res = $"{server}/{channel}";
            else if (!string.IsNullOrEmpty(platform))
                res = $"{server}/{platform}";
            else
                res = $"{server}";

            return RegularPath(res);
        }

        #region EditorPrefs
        public void SetBuildCommitPath(string path)
        {
            string key = $"{Application.productName}_{nameof(ResourceDescriptionWindow)}_BuildCommitPath";
            EditorPrefs.SetString(key, path);
        }
        public string GetBuildCommitPath()
        {
            string key = $"{Application.productName}_{nameof(ResourceDescriptionWindow)}_BuildCommitPath";
            return EditorPrefs.GetString(key);
        }
        #endregion

        #region EditorStyle
        protected static bool Foldout(bool display, string title, float woffset = 0, float hoffset = 0)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f + woffset, 22f + hoffset, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }
        #endregion
    }
}
#endif
