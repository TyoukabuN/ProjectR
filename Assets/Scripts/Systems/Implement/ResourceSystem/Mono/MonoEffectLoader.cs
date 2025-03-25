using UnityEngine;
using System;
using Sirenix.OdinInspector;
using static PJR.Systems.ResourceSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PJR.Systems
{
    [HelpURL("https://docs.qq.com/doc/DY3BsdUlVSGpLUk5W")]
    [ExecuteInEditMode]
    public class MonoEffectLoader : PrefabLoader
    {
        [SerializeField, EffectConfigID, OnValueChanged("OnEffectIDChange")]
        private int effectID;
        public int EffectID {
            get => effectID;
            set {
                if (effectID != value)
                    dirty = true;
                effectID = value;
            }
        }

        [NonSerialized]
        public EffectConfig.ConfigItem config = null;
        public EffectConfig.ConfigItem Config
        {
            get
            {
                if (config == null)
                    config = EffectConfig.GetEffectConfig(effectID);
                else if (config.ID != effectID)
                    config = EffectConfig.GetEffectConfig(effectID);
                return config;
            }
        }
        [NonSerialized]
        public ParticleSystem mainParticlSystem;
        [NonSerialized]
        public GameObject mainGameObject;
        [NonSerialized]
        public Transform mainTransform;
        [NonSerialized]
        public EffectSystem.EffectHandle effectHandle;
        public override void Release()
        {
            base.Release();

            if (effectHandle != null)
            { 
                effectHandle.Release();
                effectHandle = null;
            }
        }
        protected override bool ShouldLoad()
        {
            if (!string.IsNullOrEmpty(Error) && !dirty)
                return false;
            if (dirty)
                return true;
            if (effectHandle != null)
                return false;
            return instance == null;
        }
        protected override void Load()
        {
            if (effectID <= 0)
                return;
            if (effectHandle != null)
            { 
                effectHandle.Release();
                effectHandle = null;
            }
            state = LoadState.Loading;

            effectHandle = EffectSystem.LoadEffectByID(effectID,true);
            effectHandle.transformSpace = EffectSystem.EffectParams.TransformSpace.Local;
            effectHandle.parent = transform;
            effectHandle.Completed = OnEffectLoadDone;
            effectHandle.Load();
        }
        protected override void OnLoadDone(ResourceLoader handle) { }
        protected virtual void OnEffectLoadDone(EffectSystem.EffectHandle handle)
        {

            this.Error = effectHandle.Error;
            if (handle.State == EffectSystem.EffectHandle.HandleState.Failed)
            { 
                state = LoadState.Failure;
                LogError("[VisualEffectLoader.OnLoadDone] Fail to load effect");
                return;
            }
            //加载期间id发生变化,即release
            if (handle.effectID != effectID)
            {
                handle.Release();
                return;
            }
            //
            mainParticlSystem = handle.mainParticleSystem;
            mainGameObject = handle.gameObject;
            mainTransform = handle.transform;

            state = LoadState.Done;
        }

#if UNITY_EDITOR
        private ParticleSystem prev_particleSystem = null;
        protected override void Editor_OnDisable()
        {
            base.Editor_OnDisable();
            if (prev_particleSystem != null)
            {
                prev_particleSystem.time = 0;
            }
        }

        protected override void Editor_Update()
        {
            if (Editor_ShouldGenPreview())
            {
                Editor_GenPreviewInstance();
                return;
            }
            if (prev_particleSystem != null)
            { 
                prev_particleSystem.Simulate(Time.deltaTime, true, prev_particleSystem.time <= 0, true);
            }
        }
        void OnEffectIDChange()
        {
            editor_dirty = true;
            if (Config != null)
            {
                gameObject.name = $"VisualEffectLoader[{Config.ID}][{Config.AssetName}]";
            }
        }
        protected override bool Editor_ShouldGenPreview()
        {
            return base.Editor_ShouldGenPreview();
        }
        protected override void Editor_GenPreviewInstance(string assetName = null)
        {
            if (Config == null)
                return;
            base.Editor_GenPreviewInstance(Config.AssetName);
            prev_particleSystem = transform.GetComponentInChildren<ParticleSystem>();
        }
#endif
    }
}
