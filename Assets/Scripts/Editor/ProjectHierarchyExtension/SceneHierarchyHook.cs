using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PJR.Editor
{
    [InitializeOnLoad]
    public static class SceneHierarchyHook
    {
        static SceneHierarchyHook()
        {
            SceneHierarchyHooks.addItemsToSceneHeaderContextMenu += AddExtraSceneHeaderContextMenuItems;
        }
        
        /// <summary>
        /// 右键菜单里加个重新加载当前场景的按钮
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="target"></param>
        static void AddExtraSceneHeaderContextMenuItems(GenericMenu menu, Scene target)
        {
            menu.AddSeparator("");
            
            var validTarget = target.isLoaded;
            if (!validTarget)
                return;
            menu.AddItem(new GUIContent("Reload Scene"), false, () =>Internal_ReloadScene(target));
        }

        /// <summary>
        /// 重新记载场景的快捷键
        /// </summary>
        [Shortcut("PJR/Editor/ReloadScene", KeyCode.R,
            ShortcutModifiers.Control | ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        public static void ReloadCurrentScene()
        {
            Internal_ReloadScene(EditorSceneManager.GetActiveScene());
        }

        static void Internal_ReloadScene(Scene target)
        {
            if (target == null || !target.isLoaded)
                return;
            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(target.path, OpenSceneMode.Single);
        }
    }
}