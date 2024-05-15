using System;
using UnityEditor;
using UnityEngine;
using InfinityCode.UltimateEditorEnhancer;

namespace LS.LSEditor
{
    [InitializeOnLoad]
    public static class AssetReferenceLeftToolbar
    {
        public static Action OnLeftClick;
        private static GUIContent content;

        static AssetReferenceLeftToolbar()
        {
            ToolbarManager.AddLeftToolbar("AssetReference", OnGUI);
        }

        private static void DrawIcon()
        {
            if (content == null)
                content = new GUIContent(LoadIcon("d_Prefab Icon"), "AssetReference");
            if (GUILayoutUtils.Button(content, Styles.appToolbarButtonLeft, GUILayout.Width(32), GUILayout.Height(18)) == ButtonEvent.click)
            {
                Event e = Event.current;

                if (e.button == 0)
                {
                    //if (OnLeftClick != null) OnLeftClick();
                    ShowContextMenu();
                }
                //else if (e.button == 1) ShowContextMenu();
            }
        }

        private static Texture LoadIcon(string name)
        {
            var icon = (Texture)EditorGUIUtility.Load(name);
            if (icon != null)
                icon.filterMode = FilterMode.Bilinear;
            return icon;
        }

        private static void OnGUI()
        {
            DrawIcon();
        }


        private static void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("显示Art资产被引用数"), LSAssetAnalysisSetting.ShowAssetRefCount, () => { LSAssetAnalysisSetting.ShowAssetRefCount = !LSAssetAnalysisSetting.ShowAssetRefCount; });
            menu.AddItem(new GUIContent("刷新引用"), false, () => { LSAssetAnalysisSetting.RefreshReferenceData(); });
            menu.AddItem(new GUIContent("文档"), false, () => {
                Application.OpenURL("https://docs.qq.com/doc/DY2JrSGhiblVRVUhH");
            });
            menu.ShowAsContext();
        }
    }
}
