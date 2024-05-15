using LS.LSEditor;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.ProjectTools
{
    [InitializeOnLoad]
    public static class ProjectAssetReferedCount
    {
        /// <summary>
        /// 需要处理的路径
        /// </summary>
        private static string[] validPathStartWith = new[]
        {
            "Assets/__LS/Art",
        };
        static ProjectAssetReferedCount()
        {
            ProjectItemDrawer.Register("DRAW_ASSET_REFERED_COUNT", DrawReferCount);
        }

        static bool IsValidPath(string assetPath)
        {
            for (int i = 0; i < validPathStartWith.Length; i++)
            {
                if (!assetPath.StartsWith(validPathStartWith[i]))
                    continue;
                return true;
            }
            return false;
        }

        private static void DrawReferCount(ProjectItem item)
        {
            if (!LS.LSEditor.LSAssetAnalysisSetting.ShowAssetRefCount)
                return;
            if (item.isFolder)
                return;
            if (!IsValidPath(item.path))
                return;
            if (!LSAssetAnalysisSetting.GetRefFinderData().GetAssetState(item.guid, out var ad))
                return;
            var str = $"被引用数:{ad.references.Count}";
            var style = GetLabelStyle();
            if(ad.references.Count <=0)
                style.normal.textColor = Color.red;
            else
                style.normal.textColor = Color.green;
            EditorGUI.LabelField(item.rect, new GUIContent(str), GetLabelStyle());
            style.normal.textColor = Color.gray;
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
