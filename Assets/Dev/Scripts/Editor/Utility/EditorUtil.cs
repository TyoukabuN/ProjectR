using InfinityCode.UltimateEditorEnhancer;
using InfinityCode.UltimateEditorEnhancer.ProjectTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

namespace PJR.Editor
{
    public static class EditorUtil
    {
        public static class Build
        {
            private const string TempDir = "Temp/PlayerScriptCompilationTests";

            public static void BuildTargetCompiles(BuildTarget buildTarget)
            {
                var settings = new ScriptCompilationSettings
                {
                    group = BuildPipeline.GetBuildTargetGroup(buildTarget),
                    target = buildTarget,
                    options = ScriptCompilationOptions.None
                };

                PlayerBuildInterface.CompilePlayerScripts(settings, TempDir + "_" + buildTarget);
            }
            public static void BuildCurrentTargetCompiles()
            {
                BuildTargetCompiles(EditorUserBuildSettings.activeBuildTarget);
            }
        }
        public static class Asset
        {
            public static bool OpenScriptOfType(System.Type type)
            {
                var mono = MonoScriptFromType(type);
                if (mono == null)
                    return false;

                AssetDatabase.OpenAsset(mono);
                return true;
            }

            public static MonoScript MonoScriptFromType(System.Type targetType)
            {
                if (targetType == null) return null;
                var typeName = targetType.Name;
                if (targetType.IsGenericType)
                {
                    targetType = targetType.GetGenericTypeDefinition();
                    typeName = typeName.Substring(0, typeName.IndexOf('`'));
                }

                return AssetDatabase.FindAssets(string.Format("{0} t:MonoScript", typeName))
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                    .FirstOrDefault(m => m != null && m.GetClass() == targetType);
            }
        }
    }
    public class AssetProcessor : AssetModificationProcessor
    {
        public static event Action<string> OnWillCreateAssetCall;
        public static event Action<string, RemoveAssetOptions> OnWillDeleteAssetCall;
        public static event Action<string, string> OnWillMoveAssetCall;
        static void OnWillCreateAsset(string assetPath)
        {
            try
            {
                OnWillCreateAssetCall?.Invoke(assetPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            try
            {
                OnWillDeleteAssetCall?.Invoke(assetPath, options);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            AssetDeleteResult result = AssetDeleteResult.DidNotDelete;
            return result;
        }
        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            try
            {
                OnWillMoveAssetCall?.Invoke(sourcePath, destinationPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            AssetMoveResult assetMoveResult = AssetMoveResult.DidNotMove;

            return assetMoveResult;
        }
    }
    public static class Styles
    {
        public static GUIStyle EntryWarnIconSmallTextless { get; } = new GUIStyle("CN EntryWarnIconSmall");
        public static GUIStyle EntryErrorIconSmallTextless { get; } = new GUIStyle("CN EntryErrorIconSmall");
        public static Texture Icon_Corner_On { get; } = EditorGUIUtility.IconContent("d_winbtn_mac_max").image;
        public static Texture Icon_Corner_Off { get; } = EditorGUIUtility.IconContent("d_winbtn_mac_close").image;
        public static Texture Icon_GreenCheckMark { get; } = EditorGUIUtility.IconContent("d_GreenCheckmark").image;
        public static Texture Icon_RedCrossMark { get; } = EditorGUIUtility.IconContent("winbtn_mac_close_h").image;
        public static Texture Icon_Warm { get; } = EditorGUIUtility.IconContent("d_console.warnicon.sml").image;

        public static Texture WarningIcon { get; } = EditorGUIUtility.IconContent("d_console.warnicon.sml").image;

        public static GUIStyle label;
        public static GUIStyle GetLabelStyle()
        {
            GUIStyle style = null;
            if (label == null)
            {
                label = new GUIStyle(EditorStyles.label);
                label.alignment = TextAnchor.MiddleRight;
                label.padding.right = 10;
                label.normal.textColor = Color.gray;
            }
            style = label;

            return style;
        }

        public static Texture2D GetIcon(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
        }

        private static GUIStyle _getMiddleRightLabel;
        public static GUIStyle GetMiddleRightLabel
        {
            get
            {
                if (_getMiddleRightLabel == null)
                {
                    _getMiddleRightLabel = new GUIStyle(EditorStyles.label);
                    _getMiddleRightLabel.alignment = TextAnchor.MiddleRight;
                    _getMiddleRightLabel.padding.right = 10;
                    _getMiddleRightLabel.normal.textColor = Color.gray;
                }

                return _getMiddleRightLabel;
            }
        }

    }
    public static class FileUtil
    {
        public const string MetaExtension = ".meta";
        public static bool IsMetaFile(string assetPath) => Path.GetExtension(assetPath) == MetaExtension;
        public static bool IsMetaFile(string assetPath, out string assetPathWithMetaExtension)
        {
            assetPathWithMetaExtension = assetPath;
            if (!IsMetaFile(assetPath))
                return false;
            assetPathWithMetaExtension = assetPath.Replace(MetaExtension, string.Empty);
            return true;
        }

        public static bool IsWithinDirectory(string filePath, string directoryPath)
        {
            string fullFilePath = Path.GetFullPath(filePath);
            string fullDirectoryPath = Path.GetFullPath(directoryPath);

            fullDirectoryPath = Path.GetFullPath(fullDirectoryPath + Path.DirectorySeparatorChar);

            return fullFilePath.StartsWith(fullDirectoryPath, StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizeSeparator(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }


        public const string String_Assets = "Assets";
        public static List<string> GetAllDirectories(string assetPath) => GetAllDirectories(assetPath, String_Assets);
        public static List<string> GetAllDirectories(string assetPath, string rootPath)
        {
            List<string> directories = new List<string>();
            string directoryName = assetPath;

            while (true)
            {
                //Debug.Log(directoryName);
                if (string.IsNullOrEmpty(directoryName) || directoryName == rootPath)
                    break;

                directoryName = Path.GetDirectoryName(directoryName);
                directoryName = NormalizeSeparator(directoryName);

                directories.Add(directoryName);
            }

            return directories;
        }

        public static string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return fullPath;

            fullPath = NormalizeSeparator(fullPath);

            int index_Assets = fullPath.IndexOf("Assets/");
            if (index_Assets < 0)
                return fullPath;

            return fullPath.Substring(index_Assets);
        }

    }
    public static class HyperLinkUtil
    {
        static HyperLinkUtil()
        {
            EditorGUI.hyperLinkClicked -= OnHyperLinkClicked;
            EditorGUI.hyperLinkClicked += OnHyperLinkClicked;
        }


        public const string String_Param_PingAssetPath = "pingAssetPath";
        public const string String_Param_PingGUID = "pingGUID";

        static void OnHyperLinkClicked(EditorWindow window, HyperLinkClickedEventArgs args)
        {
            if (args.hyperLinkData.TryGetValue(String_Param_PingGUID, out string guid))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)));
            }
            //直接传在args传assetPath可能会出问题，因为assetPath可能包含空格,@等特殊字符，导致参数解析失败
            //所以加了上面的传GUID
            else if (args.hyperLinkData.TryGetValue(String_Param_PingAssetPath, out string assetPath))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)));
            }
        }

        public static void LogAssetLink(UnityEngine.Object asset) =>
            LogAssetLink(AssetDatabase.GetAssetPath(asset));

        public static void LogAssetLink(string assetPath) => Debug.Log(GetAssetLink(assetPath));

        public static string GetAssetLink(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
                return null;
            return $"<a {String_Param_PingGUID}=\"{guid}\">[{Path.GetFileName(assetPath)}]</a> {assetPath}";
        }
    }
    public static class GUIUtil
    {
        public static void EventCheck(Rect rect, EventType eventType, Action<Event> callback) => EventCheck(rect, eventType, callback, false, false);
        public static void EventCheck(Rect rect, EventType eventType, Action<Event> callback, bool ctrl, bool alt)
        {
            if (ctrl && !Event.current.control)
                return;
            if (alt && !Event.current.alt)
                return;
            if (Event.current.type == eventType && rect.Contains(Event.current.mousePosition))
                callback?.Invoke(Event.current);
        }
        public static bool EventCheck(Rect rect, EventType eventType) => EventCheck(rect, eventType, true, false, false);
        public static bool EventCheck(Rect rect, EventType eventType, bool useEvent) => EventCheck(rect, eventType, useEvent, false, false);
        public static bool EventCheck(Rect rect, EventType eventType, bool useEvent, bool ctrl, bool alt)
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (ctrl && !Event.current.control)
                return false;
            if (alt && !Event.current.alt)
                return false;
            if (Event.current.GetTypeForControl(controlID) == eventType && rect.Contains(Event.current.mousePosition))
            {
                if (useEvent)
                    Event.current.Use();
                return true;
            }
            return false;
        }

        public const int MOUSE_BUTTON_LEFT = 0;
        public const int MOUSE_BUTTON_RIGHT = 1;
        public const int MOUSE_BUTTON_MIDDLE = 2;

        public static bool LeftClick(Rect rect) => LeftClick(rect, false, false);
        public static bool LeftClick(Rect rect, bool ctrl, bool alt) => MouseDown(rect, MOUSE_BUTTON_LEFT, ctrl, alt);
        public static bool RightClick(Rect rect) => RightClick(rect, false, false);
        public static bool RightClick(Rect rect, bool ctrl, bool alt) => MouseDown(rect, MOUSE_BUTTON_RIGHT, ctrl, alt);
        public static bool MouseDown(Rect rect, int button, bool ctrl, bool alt)
        {
            if (ctrl && !Event.current.control)
                return false;
            if (alt && !Event.current.alt)
                return false;
            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == button &&
                rect.Contains(Event.current.mousePosition)
               )
            {
                Event.current.Use();
                return true;
            }
            return false;
        }

        public static Rect GetIconRect(ProjectItem item)
        {
            return GetRect(item, 16);
        }
        public static Rect GetLabelRect(ProjectItem item, string content) =>
            GetLabelRect(item, TempContent.Get(content));
        public static Rect GetLabelRect(ProjectItem item, string content, GUIStyle style) =>
            GetLabelRect(item, TempContent.Get(content), style);
        public static Rect GetLabelRect(ProjectItem item, GUIContent content)=>
            GetLabelRect(item, content, Styles.GetLabelStyle());
        public static Rect GetLabelRect(ProjectItem item, GUIContent content, GUIStyle style)
        {
            var size = style.CalcSize(content);
            return GetRect(item, size.x);
        }
        public static Rect GetRect(ProjectItem item, float width)
        {
            Rect r = item.rect;
            r.xMin = r.xMax - width;
            r.height = 16;

            item.rect.xMax -= width;
            return r;
        }
        public static Rect GetRect(ProjectItem item, float width, float height)
        {
            Rect r = item.rect;
            r.xMin = r.xMax - width;
            r.height = height;

            item.rect.xMax -= width;
            return r;
        }

        public static Rect GetZeroRect(ProjectItem item) => GetRect(item, 0);
        public static Rect GetRectUnion(Rect rect1, Rect rect2)
        {
            float xMin = Mathf.Min(rect1.x, rect2.x);
            float yMin = Mathf.Min(rect1.y, rect2.y);
            float xMax = Mathf.Max(rect1.x + rect1.width, rect2.x + rect2.width);
            float yMax = Mathf.Max(rect1.y + rect1.height, rect2.y + rect2.height);

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

    }
}