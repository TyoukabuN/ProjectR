using System;
using UnityEngine;

namespace PJR.Config
{
    public class IndexedString : OrdinalConfig<IndexedStringAsset,IndexedStringItemAsset>
    {
        private static IndexedString _instance;
        public static IndexedString Instance => _instance ??= new IndexedString();
        
        

#if UNITY_EDITOR

        public override void Editor_OpenMenuEditorWindow()
        {
            MenuEditorWindow.OpenWindow();
        }

        public override void Editor_OpenItemCreateWindow(Action<IndexedStringItemAsset> onFinish, string directory = null)
        {
            ItemCreateWindow.OpenWindow();
        }

        public class ItemCreateWindow : OrdinalConfigItemCreateWindow<IndexedString, IndexedStringAsset, IndexedStringItemAsset>
        {
            public static void OpenWindow()
            {
                var window = GetWindow<ItemCreateWindow>();
                window.Init(Instance);
                window.titleContent = new GUIContent("Create IndexedString Item");
                window.minSize = new Vector2(400, 500);
            }
        }
        public class MenuEditorWindow : OrdinalConfigMenuEditorWindow<IndexedString, IndexedStringAsset, IndexedStringItemAsset>
        {
            [RequireConfigMenuItem("配置窗口/IndexedString")]
            public static void OpenWindow()
            {
                var window = GetWindow<MenuEditorWindow>();
                window.Init(Instance);
            }
            public override void Init()=> Init(Instance);
        }

        public class Selector : OrdinalConfigSelector<IndexedString, IndexedStringAsset, IndexedStringItemAsset>
        {
            public static void Show(Action<IndexedStringItemAsset> onChanged)
            {
                var window = GetWindow<Selector>();
                window.ClearCallBacks();
                window.Init(Instance, onChanged);
            }
        }
#endif
    }
}
