using System;
using UnityEditor;
using UnityEngine;
using InfinityCode.UltimateEditorEnhancer;
using Sirenix.Utilities.Editor;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Build.Player;

namespace PJR.Editor
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
            menu.AddItem(new GUIContent("Asset引用相关/显示资产被引用数"), AssetAnalysisSetting.ShowAssetRefCount, () => { AssetAnalysisSetting.ShowAssetRefCount = !AssetAnalysisSetting.ShowAssetRefCount; });
            menu.AddItem(new GUIContent("Asset引用相关/显示是否被外部引用"), AssetAnalysisSetting.ShowIsReferedByOutside, () => { AssetAnalysisSetting.ShowIsReferedByOutside = !AssetAnalysisSetting.ShowIsReferedByOutside; });
            menu.AddItem(new GUIContent("Asset引用相关/刷新引用"), false, () => { AssetAnalysisSetting.RefreshReferenceData(); });
            menu.AddItem(new GUIContent("Asset引用相关/刷新引用[并清理缓存]"), false, () => { AssetAnalysisSetting.ClearCacheAndRefreshReferenceData(); });
            menu.AddItem(new GUIContent("Asset引用相关/Ping当前外部引用检查目录"), false, () => { AssetAnalysisSetting.PingRootForOutsideReferedCheck(); });
            menu.AddItem(new GUIContent("Asset引用相关/设置需要显示引用数的路径"), false, () => { AssetAnalysisSetting.GetAsset(); AssetAnalysisSetting.OpenAsset(); });
            //
            menu.AddItem(new GUIContent("Debug/Setting/ResourceSystem.DebugLevel/0"), ResourceSystem.DebugLevel == 0, () => { ResourceSystem.DebugLevel = 0; });
            menu.AddItem(new GUIContent("Debug/Setting/ResourceSystem.DebugLevel/1"), ResourceSystem.DebugLevel == 1, () => { ResourceSystem.DebugLevel = 1; });
            menu.AddItem(new GUIContent("Debug/编译测试"), false, () => { EditorUtility.Build.BuildCurrentTargetCompiles(); });
            //
            menu.AddItem(new GUIContent("编辑菜单脚本"), false, () => { EditorUtility.Asset.OpenScriptOfType(typeof(AssetReferenceLeftToolbar)); });
            menu.AddItem(new GUIContent("文档"), false, () => {
                Application.OpenURL("https://docs.qq.com/doc/DY2JrSGhiblVRVUhH");
            });
            menu.ShowAsContext();
        }
    }
}

