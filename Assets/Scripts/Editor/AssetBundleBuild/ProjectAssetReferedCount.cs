using InfinityCode.UltimateEditorEnhancer;
using InfinityCode.UltimateEditorEnhancer.ProjectTools;
using UnityEditor;
using UnityEngine;

namespace PJR.Editor
{
    [InitializeOnLoad]
    public static class ProjectAssetReferedCount
    {
        static ProjectAssetReferedCount()
        {
            ProjectItemDrawer.Register("DRAW_ASSET_REFERED_COUNT", DrawAssetReferCounts);
            ProjectItemDrawer.Register("SetRootForMessReferCheck", DrawButton, 20);
        }

        static bool IsValidPath(string assetPath)
        {
            var referedCountCheckPath = AssetAnalysisSetting.ReferedCountCheckPath;
            for (int i = 0; i < referedCountCheckPath.Length; i++)
            {
                if (!assetPath.StartsWith(referedCountCheckPath[i]))
                    continue;
                return true;
            }
            return false;
        }

        private static void DrawAssetReferCounts(ProjectItem item)
        {
            Rect rect = item.rect;
            DrawGlobalReferCount(item);
            DrawOutsideRefered(item);
        }
        private const float pixelPerChar = 11f;
        //全局的被引用计数
        private static void DrawGlobalReferCount(ProjectItem item)
        {
            if (!AssetAnalysisSetting.ShowAssetRefCount)
                return;
            if (item.isFolder)
                return;
            if (!IsValidPath(item.path))
                return;
            if (!AssetAnalysisSetting.GetRefFinderData().GetAssetState(item.guid, out var ad))
                return;
            var str = $"[被引用数:{ad.references.Count}]";
            var style = GetLabelStyle();
            if (ad.references.Count <= 0)
                style.normal.textColor = Color.red;
            else
                style.normal.textColor = Color.green;

            float strlen2Pixel = str.Length * pixelPerChar;
            Rect r = item.rect;
            r.xMin = r.xMax - strlen2Pixel;
            item.rect.xMax -= strlen2Pixel;

            EditorGUI.LabelField(r, new GUIContent(str), GetLabelStyle());
            style.normal.textColor = Color.gray;
        }
        private static void DrawOutsideRefered(ProjectItem item)
        {
            if (!AssetAnalysisSetting.ShowIsReferedByOutside)
                return;
            if (item.isFolder)
                return;
            if (!AssetAnalysisSetting.GetRefFinderData().GetAssetState(item.guid, out var ad))
                return;
            if (string.IsNullOrEmpty(AssetAnalysisSetting.RootForOutsideReferedCheck))
                return;
            string dir = AssetAnalysisSetting.RootForOutsideReferedCheck;
            if (!item.path.Contains(dir))
                return;

            dir = PathUtil.RegularPath(dir);
            //var pInfo_dir = new PathUtil.PathInfo(dir);

            bool isMessRef = false;
            foreach (var reference in ad.references)
            {
                var _path = AssetDatabase.GUIDToAssetPath(reference);
                if (string.IsNullOrEmpty(_path))
                    continue;
                if (!_path.Contains(dir))
                {
                    isMessRef = true;
                    break;
                }
            }

            string str = null;
            var style = GetLabelStyle();
            if (isMessRef)
            {
                str = $"[存在外部引用]";
                style.normal.textColor = Color.red;
            }
            else
            {
                str = $"[无外部引用]";
                style.normal.textColor = Color.green;
            }

            float strlen2Pixel = str.Length * pixelPerChar;
            Rect r = item.rect;
            r.xMin = r.xMax - strlen2Pixel;
            item.rect.xMax -= strlen2Pixel;

            EditorGUI.LabelField(r, new GUIContent(str), GetLabelStyle());
            style.normal.textColor = Color.gray;
        }

        private static void DrawButton(ProjectItem item)
        {
            if (!AssetAnalysisSetting.ShowIsReferedByOutside) return;
            if (!item.isFolder) return;
            if (!item.path.StartsWith("Assets")) return;

            bool isMatch = AssetAnalysisSetting.RootForOutsideReferedCheck == item.path;

            if (isMatch)
            {
                Rect r = GetIconRect(item);
                if (GUI.Button(r, TempContent.Get(EditorGUIUtility.IconContent("d_DebuggerAttached").image, "取消作为外部引用检查的根目录"), GUIStyle.none))
                {
                    AssetAnalysisSetting.RootForOutsideReferedCheck = string.Empty;
                }
                return;
            }

            if (item.hovered)
            {
                Rect r = GetIconRect(item);
                if (GUI.Button(r, TempContent.Get(EditorGUIUtility.IconContent("d_DebuggerDisabled").image, "作为外部引用检查的根目录"), GUIStyle.none))
                {
                    AssetAnalysisSetting.RootForOutsideReferedCheck = item.path;
                }
            }
        }

        private static Rect GetIconRect(ProjectItem item)
        {
            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;
            return r;
        }

        static GUIStyle label;
        static GUIStyle GetLabelStyle()
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
    }
}
