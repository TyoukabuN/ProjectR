#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System;
using PJR.Systems;

namespace PJR
{
    public partial class PrefabLoader : MonoBehaviour
    {
        public const string Prefix_preview = "[Preview]";
        [NonSerialized]
        protected bool editor_dirty = true;
        [NonSerialized]
        protected GameObject prev_gobj = null;

        protected virtual void Editor_OnEnable()
        {
            if (EditorApplication.isPlaying) 
                return;
            Editor_DealEditorEvent(true);
        }
        protected virtual void Editor_Update()
        {
            if (EditorApplication.isPlaying) 
                return;
            if (Editor_ShouldGenPreview())
                Editor_GenPreviewInstance();
        }
        protected virtual void Editor_OnDisable()
        {
            if (EditorApplication.isPlaying) 
                return;
            Editor_DealEditorEvent(false);
        }
        protected virtual void Editor_OnDestroy()
        {
            if (EditorApplication.isPlaying) 
                return;
            Editor_DealEditorEvent(false);
        }
        protected virtual void Editor_DealEditorEvent(bool register)
        {
            if (register)
            {
                Editor_DealEditorEvent(false);
                EditorApplication.playModeStateChanged += Editor_OnPlayModeStateChanged;
                PrefabStage.prefabSaving += Editor_OnPrefabSaving;
                PrefabStage.prefabStageClosing += Editor_OnPrefabStageClosing;

            }
            else
            {
                EditorApplication.playModeStateChanged -= Editor_OnPlayModeStateChanged;
                PrefabStage.prefabSaving -= Editor_OnPrefabSaving;
                PrefabStage.prefabStageClosing -= Editor_OnPrefabStageClosing;
            }

        }
        protected virtual void Editor_OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                Editor_ClearPreview();
        }
        protected virtual void Editor_OnPrefabSaving(GameObject gobj)
        {
            if (prev_gobj != null)
                Editor_ClearPreview();
        }
        protected virtual void Editor_OnPrefabStageClosing(PrefabStage prefabStage)
        {
            if (prev_gobj != null)
                Editor_ClearPreview();
        }

        protected virtual void Editor_GenPreviewInstance(string assetName = null)
        {
            editor_dirty = false;
            Editor_ClearPreview();
            //
            assetName = !string.IsNullOrEmpty(assetName) ? assetName : AssetName;
            if (!CheckAssetName(assetName))
                return;
            var prefab = ResourceSystem.EditorLoadAsset<GameObject>(assetName);
            if (prefab == null)
                return;
            var temp = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            temp.name = $"{Prefix_preview}{temp.name}";
            //temp.tag = "EditorOnly";
            temp.hideFlags = HideFlags.DontSave | HideFlags.DontSaveInEditor;

            temp.transform.SetParent(transform, false);
            temp.transform.localPosition = Vector3.zero;
            temp.transform.rotation = Quaternion.identity;
            temp.transform.localScale = Vector3.one;

            prev_gobj = temp;
        }

        protected virtual bool Editor_ShouldGenPreview()
        {
            if (!Previewable)
                return false;
            if (prev_gobj == null)
                return true;
            if (editor_dirty)
                return true;
            return false;
        }

        protected virtual void Editor_ClearPreview()
        {
            if (prev_gobj != null)
                DestroyImmediate(prev_gobj);
            foreach (Transform t in transform.GetChilds())
            {
                if (t.gameObject.name.StartsWith(Prefix_preview))
                    DestroyImmediate(t.gameObject);
            }
            prev_gobj = null;
        }

        /// <summary>
        /// 能不能预览
        /// </summary>
        protected virtual bool Previewable
        {
            get
            {
                if (EditorApplication.isPlaying)
                    return false;
                if (!Selected)
                    return false;
                return !EditorApplication.isPlaying;
            }
        }
        /// <summary>
        /// 是否被选中
        /// </summary>
        protected virtual bool Selected
        {
            get
            {
                if (Selection.activeGameObject == null)
                    return false;
                return Selection.activeGameObject == this.gameObject;
            }
        }
    }
}
#endif
