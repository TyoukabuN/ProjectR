using System;
using UnityEngine;

namespace PJR.Config
{
    public class AvatarConfig : OrdinalConfig<AvatarConfigAsset,AvatarConfigItemAsset>
    {
        private static AvatarConfig _instance;
        public static AvatarConfig Instance => _instance ??= new AvatarConfig();

        public override void Editor_OpenItemCreateWindow(Action<AvatarConfigItemAsset> onFinish, string directory = null)
        {
            ItemCreateWindow.OpenWindow();
        }

#if UNITY_EDITOR
        public class ItemCreateWindow : OrdinalConfigItemCreateWindow<AvatarConfig, AvatarConfigAsset, AvatarConfigItemAsset>
        {
            public static void OpenWindow()
            {
                var window = GetWindow<ItemCreateWindow>();
                window.Init(Instance);
                window.titleContent = new GUIContent("Create AvatarConfig Item");
                window.minSize = new Vector2(400, 500);
            }
        }
        public class MenuEditorWindow : OrdinalConfigMenuEditorWindow<AvatarConfig, AvatarConfigAsset, AvatarConfigItemAsset>
        {
            [RequireConfigMenuItem("配置窗口/AvatarConfig")]
            public static void OpenWindow()
            {
                var window = GetWindow<MenuEditorWindow>();
                window.Init(Instance);
            }
            public override void Init()=> Init(Instance);
        }

        public class Selector : OrdinalConfigSelector<AvatarConfig, AvatarConfigAsset, AvatarConfigItemAsset>
        {
        }
#endif
    }
}