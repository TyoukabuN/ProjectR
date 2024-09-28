using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJR
{
    public partial class ResourceSystem
    {
        public class ResourceLoader : AsyncLoader
        {
            public bool IsEditor { get; protected set; } = false;
            public string AssetName { get; protected set; }
            public string AssetFullName { get; protected set; }
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

            public ResourceLoader(string assetFullName, Type assetType)
            {
                this.AssetFullName = assetFullName;
                this.AssetType = assetType;
                //
                AssetName = Path.GetFileName(assetFullName);
            }

            public virtual Object GetRawAsset()
            {
                //TODO:加个实例化，加引用计数
                return AssetObject;
            }
            public virtual T GetRawAsset<T>() where T : UnityEngine.Object
            {
                return (T)AssetObject;
            }

            public List<UnityEngine.Object> _instantiates = new List<UnityEngine.Object>(12);
            public virtual T GetInstantiate<T>() where T : UnityEngine.Object
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

