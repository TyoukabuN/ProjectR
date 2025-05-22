#if UNITY_EDITOR
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PJR.Config
{
    public class __OrdinalConfig : OrdinalConfig<__OrdinalConfigAsset,__OrdinalConfigItemAsset>
    {
        private static __OrdinalConfig _instance;
        public static __OrdinalConfig Instance => _instance ??= new __OrdinalConfig();

#if UNITY_EDITOR
        public override void Editor_OpenItemCreateWindow(Action<__OrdinalConfigItemAsset> onFinish, string directory = null)
        {
            ItemCreateWindow.OpenWindow();
        }

        public class ItemCreateWindow : OrdinalConfigItemCreateWindow<__OrdinalConfig, __OrdinalConfigAsset, __OrdinalConfigItemAsset>
        {
            public static void OpenWindow()
            {
                var window = GetWindow<ItemCreateWindow>();
                window.Init(Instance);
                window.titleContent = new GUIContent("Create __OrdinalConfig Item");
                window.minSize = new Vector2(400, 500);
            }
        }
        public class MenuEditorWindow : OrdinalConfigMenuEditorWindow<__OrdinalConfig, __OrdinalConfigAsset, __OrdinalConfigItemAsset>
        {
            [RequireConfigMenuItem("配置窗口/__OrdinalConfig")]
            public static void OpenWindow()
            {
                var window = GetWindow<MenuEditorWindow>();
                window.Init(Instance);
            }
            public override void Init()=> Init(Instance);
        }

        public class Selector : OrdinalConfigSelector<__OrdinalConfig, __OrdinalConfigAsset, __OrdinalConfigItemAsset>
        {
            public static void Show(Action<__OrdinalConfigItemAsset> onChanged)
            {
                var window = GetWindow<Selector>();
                window.ClearCallBacks();
                window.Init(Instance, onChanged);
            }
        }
#endif
    }
}
#endif