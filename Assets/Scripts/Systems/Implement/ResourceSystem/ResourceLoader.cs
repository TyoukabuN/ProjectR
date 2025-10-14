using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PJR.Core;
using Object = UnityEngine.Object;

namespace PJR.Systems
{
    public partial class ResourceSystem
    {
        public abstract class ResourceLoader<T> : ResourceLoader where T : ResourceLoader
        {
            protected ResourceLoader(string assetPath, Type assetType) : base(assetPath, assetType)
            {
            }
        }

        public abstract class ResourceLoader : AsyncLoader
        {
            public bool IsEditor { get; protected set; } = false;
            public string AssetName { get; protected set; }
            public string AssetPath { get; protected set; }
            public Object AssetObject { get; protected set; }
            public Type AssetType { get; protected set; }

            public Action<ResourceLoader> Completed;

            public enum LoaderState
            {
                None,
                DownloadingAssetBundle,
                LoadingAssetBundle,
                LoadindAsset,
                Error,
                Done,
                Released,
            }
            public LoaderState State { get; protected set; } = LoaderState.None;
            public bool isDone => State == LoaderState.Done;
            public bool isReleased => State == LoaderState.Released;

            public ResourceLoader(string assetPath, Type assetType)
            {
                AssetPath = assetPath;
                AssetName = Path.GetFileName(assetPath);
                AssetType = assetType;
            }
            public virtual Object GetRawAsset()
            {
                //TODO:加个实例化，加引用计数
                return AssetObject;
            }
            public virtual T GetRawAsset<T>() where T : Object
            {
                return (T)AssetObject;
            }

            public List<Object> _instantiates = new List<Object>(12);
            public virtual T GetInstantiate<T>() where T : Object
            {
                var inst = Instantiate<T>(GetRawAsset<T>());
                if (inst == null)
                    return null;
                _instantiates.Add(inst);
                return inst;
            }

            public virtual void ClearNullInstantiate()
            {
                if (_instantiates == null || _instantiates.Count <= 0)
                    return;
                for (int i = 0; i < _instantiates.Count; i++)
                {
                    if (_instantiates[i] == null)
                    { 
                        _instantiates.RemoveAt(i--);
                        continue;
                    }
                }
            }
            public virtual bool Releasable()
            {
                if (_instantiates.Count > 0)
                    return false;
                return true;
            }
     
            public override void Release()
            {
                this.State = LoaderState.Released;
            }
        }

        public abstract class AsyncLoader : IEnumerator
        {
            public object Current => null;
            public bool MoveNext() => !IsDone();
            public virtual void Reset() { }
            public virtual void Release() { }
            public virtual bool IsDone() => true;
            public virtual void Update() { }
            public virtual string error { get; protected set; } = string.Empty;
        }
    }
}

