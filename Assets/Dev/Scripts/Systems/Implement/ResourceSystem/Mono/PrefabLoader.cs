using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using System;
using static PJR.Systems.ResourceSystem;
using PJR.Systems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    [ExecuteInEditMode]
    public partial class PrefabLoader : MonoBehaviour
    {
        public enum LoadState : int
        {
            None = 0,
            Loading,
            Failure,
            Done,
            Release,
        }

        public const string Ext_prefab = ".prefab";

        [PrefabNameValidate, HideIf("@GetType() == typeof(VisualEffectLoader)")]
        public string AssetName = string.Empty;

        [NonSerialized]
        protected bool dirty = false;
        [NonSerialized]
        protected ResourceLoader resourceLoader = null;
        [NonSerialized]
        protected GameObject instance = null;
        public GameObject Instance => instance;

        [NonSerialized, HideIf("@string.IsNullOrEmpty(Error)")]
        public string Error = string.Empty;

        protected bool Released => state == LoadState.Release;
        [NonSerialized]
        protected LoadState state = LoadState.None;

        [ShowInInspector]
        public LoadState State => LoadState.None;

#if UNITY_EDITOR
        [OnInspectorGUI, PropertyOrder(1000)]
        public void ShowStateTips()
        {
            if(!string.IsNullOrEmpty(Error)) EditorGUILayout.HelpBox(Error, MessageType.Error);
        }
#endif

        protected virtual void Update()
        {
            if (Application.isPlaying)
            {
                if(ShouldLoad())
                    Load();
            }
#if UNITY_EDITOR
            Editor_Update();
#endif
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            Editor_OnEnable();
#endif
        }
        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            Editor_OnDisable();
#endif
        }

        protected virtual void OnDestroy()
        {
            Release();
#if UNITY_EDITOR
            Editor_OnDestroy();
#endif
        }

        public virtual void Release()
        {
            state = LoadState.Release;
            if (instance != null)
            {
                GameObject.DestroyImmediate(instance);
                instance = null;
            }
            resourceLoader?.Release();
        }
        public virtual void SetDirty()
        {
            dirty = true;
        }
        protected virtual bool ShouldLoad()
        {
            if (!string.IsNullOrEmpty(Error) && !dirty)
                return false;
            if (dirty)
                return true;
            return state < LoadState.Loading;
        }
        protected virtual void Load()
        {
            dirty = false;
            if (!CheckAssetName(AssetName, true))
                return;
            state = LoadState.Loading;

            resourceLoader = ResourceSystem.LoadAsset(AssetName, typeof(GameObject));
            resourceLoader.Completed += OnLoadDone;
        }
        protected virtual void OnLoadDone(ResourceLoader loader)
        {
            state = LoadState.Failure;

            if (loader.State == ResourceLoader.LoaderState.Error)
            {
                LogError($"[PrefabLoader.OnLoadDone] Fail to load asset: {AssetName}",gameObject);
                return;
            }
            if (loader.AssetName != AssetName)
            {
                loader.Release();
                return;
            }
            if (loader.AssetObject == null)
                return;

            instance = Instantiate((GameObject)loader.AssetObject) as GameObject;
            state = LoadState.Done;
        }
        public virtual bool CheckAssetName(string assetName = null, bool log = false)
        {
            assetName = !string.IsNullOrEmpty(assetName) ? assetName : AssetName;

            if (string.IsNullOrEmpty(assetName))
            {
                if (log) LogError($"[assetName为空]", gameObject);
                return false;
            }
            string ext = Path.GetExtension(assetName);
            if (string.IsNullOrEmpty(ext))
            {
                if (log) LogError($"[后缀不能为空]", gameObject);
                return false;
            }
            else if (ext != Ext_prefab)
            {
                if (log) LogError($"[后缀不是: {Ext_prefab}]", gameObject);
                return false;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ResourceSystem.Editor_TryGetAssetInfoByName(assetName, out var assetInfo);
                if (!string.IsNullOrEmpty(assetInfo.Error))
                {
                    if (log) LogError($"[找不到对应Asset]", gameObject);
                    return false;
                }
            }
#endif
            return true;
        }

        protected virtual void LogError(object message, UnityEngine.Object context = null)
        {
            Error = message.ToString();
            Debug.LogError(message, context);
        }
    }

}
