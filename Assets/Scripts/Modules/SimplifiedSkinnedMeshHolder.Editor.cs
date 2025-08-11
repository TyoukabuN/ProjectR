#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR
{
    public partial class SimplifiedSkinnedMeshHolder
    {
        public class SimplifiedSetting
        {
            [PropertyRange(0f, 1f)]
            public float Quality = 0.1f;
            public bool IsReadable = true;
            [HideIf("@true")]
            public OdinEditorWindow odinEditorWindow;

            private Mesh mesh;
            [Button]
            public void Gen()
            {
                Editor_GenerateSimplifiedMeshAndSave(mesh, Quality);
                odinEditorWindow?.Close();
            }
            public SimplifiedSetting(Mesh mesh)
            {
                this.mesh = mesh;
            }
        }

        [MenuItem("CONTEXT/SkinnedMeshRenderer/生成减面Mesh并保存为Asset")]
        public static void Editor_CONTEXT_GenerateSimplifiedMeshAndSave(MenuCommand command)
        {
            var skinnedMeshRenderer = (SkinnedMeshRenderer)command?.context;
            var mesh = skinnedMeshRenderer?.sharedMesh;
            if (mesh == null)
            {
                Debug.LogWarning("SkinnedMeshRenderer没有Mesh!");
                return;
            }
            var setting = new SimplifiedSetting(mesh);
            setting.odinEditorWindow = OdinEditorWindow.InspectObject(setting);
        }

        [MenuItem("Assets/生成减面Mesh并保存为Asset")]
        static void Editor_Assets_GenerateSimplifiedMeshAndSave()
        {
            Mesh mesh = Selection.activeObject as Mesh;
            if (mesh == null)
            {
                Debug.LogWarning("选中Asset不是Mesh!");
                return;
            }
            var setting = new SimplifiedSetting(mesh);
            setting.odinEditorWindow = OdinEditorWindow.InspectObject(setting);
        }
        [MenuItem("Assets/生成减面Mesh并保存为Asset", validate = true)]
        static bool Editor_Assets_GenerateSimplifiedMeshAndSave_Validate()
        {
            var obj = Selection.activeObject;
            return obj != null && obj is Mesh;
        }

    }
}

#endif
