#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.ProjectTools
{
    [InitializeOnLoad]
    public static class ProjectTortoiseGitMenu
    {
        static ProjectTortoiseGitMenu()
        {
            ProjectItemDrawer.Register("TORTOISE_GIT_MENU", DrawMenu, 100);
        }

        private static Texture2D _tortoiseGitThumb;
        private static string _tortoiseGitThumbPath = "Assets/Dev/Scripts/Utility/TortoiseGitUtil/Resources/Icons/TortoiseGitThumb.png";
        public static Texture2D TortoiseGitThumb
        {
            get
            {
                if (_tortoiseGitThumb == null)
                    _tortoiseGitThumb = AssetDatabase.LoadAssetAtPath<Texture2D>(_tortoiseGitThumbPath);
                return _tortoiseGitThumb;
            }
        }
        private static void DrawMenu(ProjectItem item)
        {
            if (!item.isFolder) return;
            if (!item.hovered) return;
            if (!item.path.StartsWith("Assets")) return;

            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;

            ButtonEvent be = GUILayoutUtils.Button(r, TempContent.Get(TortoiseGitThumb, "TortoiseGit Menu"), GUIStyle.none);

            if (be == ButtonEvent.click)
            {
                Event e = Event.current;
                if (e.button == 0)
                {
                    Object asset = item.asset;

                    GenericMenuEx menu = GenericMenuEx.Start();

                    string itemFullPath = PJR.PathUtility.GetFullPath(item.path);

                    menu.Add("Log", () => TortoiseGitUtil.SVNLog(itemFullPath));
                    menu.Add("Commit", () => TortoiseGitUtil.SVNCommit(itemFullPath));
                    menu.Add("Revert", () => TortoiseGitUtil.SVNRevert(itemFullPath));
                    menu.Add("Pull", () => TortoiseGitUtil.SVNPull(itemFullPath));
                    menu.Add("Push", () => TortoiseGitUtil.SVNPush(itemFullPath));
                    menu.Add("需要安装TortoiseGit", () => { });

                    menu.Show();
                }
            }
        }
    }
}
#endif