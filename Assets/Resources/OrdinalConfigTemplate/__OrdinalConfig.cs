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
        public override void Editor_OpenMenuEditorWindow()
            => MenuEditorWindow.OpenWindow();
        public override void Editor_OpenItemCreateWindow(Action<__OrdinalConfigItemAsset> onFinish, string directory = null)
            => ItemCreateWindow.OpenWindow();
        public class ItemCreateWindow : OrdinalConfigItemCreateWindow<__OrdinalConfig, __OrdinalConfigAsset, __OrdinalConfigItemAsset>
        {
            public static void OpenWindow(Action<__OrdinalConfigItemAsset> onFinish = null)
            {
                var window = GetWindow<ItemCreateWindow>();
                window.Init(Instance);
                window.titleContent = new GUIContent("Create __OrdinalConfig Item");
                window.minSize = new Vector2(400, 500);
                window.onFinish = onFinish;
            }
        }
        public class MenuEditorWindow : OrdinalConfigMenuEditorWindow<__OrdinalConfig, __OrdinalConfigAsset, __OrdinalConfigItemAsset>
        {
            private static MenuEditorWindow _wnd;

            [RequireConfigMenuItem("配置窗口/__OrdinalConfig")]
            public static MenuEditorWindow OpenWindow()
            {
                _wnd?.Close();
                _wnd = GetWindow<MenuEditorWindow>();
                _wnd.Init(Instance);
                return _wnd;
            }
            public static void CloseWindow()
            {
                _wnd?.Close();
                _wnd = null;
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