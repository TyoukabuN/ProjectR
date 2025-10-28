using UnityEditor;
using UnityEngine;

namespace LS.LSEditor
{
    public static class ClearDontSaveFlag
    {
        [MenuItem("Assets/Clear HideFlags.DontSave", true)]
        private static bool ValidateClearDontSaveFlag()
        {
            // 只有选中 Object 时菜单项才可用
            return Selection.activeObject != null;
        }

        [MenuItem("Assets/Clear HideFlags.DontSave")]
        private static void ClearFlag()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj == null) continue;

                // 检查并清除 DontSave 标志
                if ((obj.hideFlags & HideFlags.DontSave) != 0)
                {
                    obj.hideFlags &= ~HideFlags.DontSave;
                    EditorUtility.SetDirty(obj);
                    Debug.Log($"✅ Cleared HideFlags.DontSave on: {AssetDatabase.GetAssetPath(obj)}");
                }
                else
                {
                    Debug.Log($"ℹ️ {AssetDatabase.GetAssetPath(obj)} has no DontSave flag.");
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}