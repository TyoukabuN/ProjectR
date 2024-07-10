using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

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
            }
            public LoaderState State { get; protected set; } = LoaderState.None;
            public bool isDone => State == LoaderState.Done;

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

