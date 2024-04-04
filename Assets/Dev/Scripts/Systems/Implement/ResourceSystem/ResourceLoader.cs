using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

namespace PJR
{
    public class ResourceLoader : AsyncLoader
    {
        public bool isEditor { get; protected set; } = false;
        public string assetName { get; protected set; }
        public string assetFullName { get; protected set; }
        public Object asset { get; protected set; }
        public Type assetType { get; protected set; }

        public Action<ResourceLoader> OnDone;

        public enum Phase { 
            None,
            DownloadingAssetBundle,
            LoadingAssetBundle,
            LoadindAsset,
            Done,
        }
        public Phase phase { get; protected set; } = Phase.None;
        public bool isDone => phase == Phase.Done;

        public ResourceLoader(string assetFullName, Type assetType)
        { 
            this.assetFullName = assetFullName; 
            this.assetType = assetType; 
            //
            assetName = Path.GetFileName(assetFullName);
        }

        public Object GetAsset()
        {
            return asset;
        }
        public T GetAsset<T>()
        {
            return (T)asset;
        }
    }

    public abstract class AsyncLoader : IEnumerator
    {
        public object Current => null;
        public bool MoveNext() => !IsDone();
        public virtual void Reset() { }
        public virtual void Dispose() { }
        public virtual bool IsDone() => true;
        public virtual void Update() { }
        public virtual string error { get; protected set; } = string.Empty;
    }
}

