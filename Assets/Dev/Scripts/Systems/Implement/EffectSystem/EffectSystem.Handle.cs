using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PJR.ResourceSystem;

namespace PJR
{
    public partial class EffectSystem
    {
        public class EffectHandle : IEnumerator
        {
            public EffectParams Params;
            public enum HandleState
            {
                None,
                Loading,
                Done,
                Released,
                Failed,
            }
            public HandleState State;

            public string Error = string.Empty;

            public GameObject gameObject = null;

            public ParticleSystem mainParticleSystem = null;
            public Transform transform => gameObject?.transform;
            public EffectConfig.ConfigItem Config => Params.Config;

            public Action<EffectHandle> Completed = null;

            private EffectHandle()
            {
                State = HandleState.None;
            }
            public EffectHandle(EffectParams @params) : this()
            {
                SetParams(@params);
            }
            public void SetParams(EffectParams @params)
            {
                Params = @params;
            }
            private void OnLoadDone_Interal(ResourceLoader resLoader)
            {
                State = HandleState.Done;

                gameObject = resLoader.GetRawAsset<GameObject>();
                Params?.Apply(gameObject);
                var callback = Params?.callback;
                if (callback != null)
                {
                    try
                    {
                        callback.Invoke(this);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[MonoEffect.EffectHandler.OnLoadDone_Interal] failed to invoked callback!");
                        Debug.Log(e.ToString());
                    }
                }
                if (Completed != null)
                {
                    try
                    {
                        Completed.Invoke(this);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[MonoEffect.EffectHandler.OnLoadDone_Interal] failed to invoked OnComplete!");
                        Debug.Log(e.ToString());
                    }
                }
            }

            private ResourceLoader resourceHandle = null;
            public bool Load()
            {
                if (State == HandleState.Done && !IsPoolOrDestroy())
                    return true;
                if (State == HandleState.Loading)
                    return true;

                State = HandleState.Loading;

                gameObject = null;

                resourceHandle = ResourceSystem.LoadAsset(effectName, typeof(GameObject));
                if (resourceHandle == null)
                {
                    Debug.LogError($"[MonoEffect.EffectHandler.Load] failed to load effect!  VisualEffectSystem.SpawnVEffect == false");
                    State = HandleState.Failed;
                    return false;
                }
                resourceHandle.Completed = OnLoadDone_Interal;

                return true;
            }
            bool IsPoolOrDestroy()
            {
                if (State == HandleState.Released)
                    return true;
                if (gameObject == null)
                    return true;
                return false;
            }
            public void SetActive(bool active)
            {
                if (active)
                {
                    if (IsPoolOrDestroy())
                    {
                        gameObject = null;
                        Load();
                    }
                    else if (!gameObject.activeSelf)
                    {
                        gameObject.SetActive(true);
                    }
                }
                else
                {
                    //gameObject?.PoolOrDestroy();
                }
            }
            public void Release()
            {
                State = HandleState.Released;
                //gameObject?.PoolOrDestroy();
                gameObject = null;
            }
            public static implicit operator GameObject(EffectHandle handle)
            {
                return handle?.gameObject;
            }

            public bool IsDone
            {
                get
                {
                    return State == HandleState.Failed || State == HandleState.Done;
                }
            }
            #region IEnumerator
            bool IEnumerator.MoveNext()
            {
                return !IsDone;
            }
            void IEnumerator.Reset()
            {
            }
            object IEnumerator.Current
            {
                get { return this; }
            }
            #endregion

            #region G/Setter
            public int effectID { get { return Params.effectID; } set { Params.effectID = value; } }
            public string effectName { get { return Params.effectName; } set { Params.effectName = value; } }
            public Action<EffectHandle> callback { get { return Params.callback; } set { Params.callback = value; } }
            void ApplyTransform() { if (Params != null) { Params.ApplyTransform(gameObject); }; }
            public void SetParent(Transform parent) { this.parent = parent; }
            public EffectParams.TransformSpace transformSpace { get { return Params.transformSpace; } set { Params.transformSpace = value; ApplyTransform(); } }
            public Transform parent { get { return Params.parent; } set { Params.parent = value; ApplyTransform(); } }
            public Vector3 position { get { return Params.position; } set { Params.position = value; ApplyTransform(); } }
            public Vector3 localPosition { get { return Params.position; } set { Params.position = value; ApplyTransform(); } }
            public Quaternion rotation { get { return Quaternion.Euler(Params.eularAngle); } set { Params.eularAngle = value.eulerAngles; ApplyTransform(); } }
            public Vector3 localScale { get { return Params.scale; } set { Params.scale = value; ApplyTransform(); } }
            #endregion
        }
        public class EffectParams
        {
            public enum TransformSpace
            {
                Local,
                World,
            }

            public string effectName = string.Empty;
            public int effectID = -1;

            public EffectConfig.ConfigItem config = null;
            public EffectConfig.ConfigItem Config
            {
                get
                {
                    if (config == null)
                    {
                        config = config ?? EffectConfig.GetEffectConfig(effectID);
                        config = config ?? EffectConfig.GetEffectConfig(effectName);
                        if (config == null)
                        {
                            if (effectName != null)
                            {
                                Debug.LogWarning($"[MonoEffect.EffectParams.Config] failed to found effect config with effectName: {effectName} !");
                                config = new EffectConfig.ConfigItem();
                                config.AssetName = effectName;
                            }
                            else
                            {
                                Debug.LogWarning($"[MonoEffect.EffectParams.Config] failed to got effect config with effectID: {effectID} !");
                            }
                        }
                    }
                    return config;
                }
                set {
                    if (value == null)
                        return;
                    if (config != value)
                    { 
                        config = value;
                        SetDirty();
                        effectID = config.ID;
                        effectName = config.AssetName;
                    }
                }
            }

            private bool dirty = false;
            public void SetDirty()
            { 
                dirty = true;
            }

            /// <summary>
            /// 放在远处来代替SetActive(false)
            /// </summary>
            public Vector3 position = Vector3.zero;
            public Vector3 eularAngle = Vector3.zero;
            public Vector3 scale = Vector3.one;

            public Transform parent = null;
            //public string boneName = string.Empty;
            //public TransformHierarchyHub attachedGetter = null;

            public TransformSpace transformSpace = TransformSpace.World;

            public Action<EffectHandle> callback = null;

            public Transform GetParent()
            {
                //if (transformHierarchyHub != null && string.IsNullOrEmpty(boneName))
                //    return transformHierarchyHub.GetBone(boneName);
                if (parent != null)
                    return parent;
                return null;
            }
            public bool TryGetParent(out Transform parent)
            {
                parent = GetParent();
                return parent != null;
            }
            public void Apply(GameObject effect)
            {
                if (effect == null)
                    return;
                ApplyTransform(effect);
            }
            public void ApplyTransform(GameObject effect)
            {
                if (effect == null)
                    return;
                var transform = effect.transform;
                var parent = GetParent();
                transform.SetParent(parent, false);

                if (transformSpace == TransformSpace.Local)
                {
                    transform.localPosition = position;
                    transform.localRotation = Quaternion.Euler(eularAngle);
                    transform.localScale = scale;
                }
                else if (transformSpace == TransformSpace.World)
                {
                    transform.position = position;
                    transform.rotation = Quaternion.Euler(eularAngle);
                    transform.localScale = scale;
                }
            }
        }
    }
}