using System;
using UnityEngine;
using YooAsset;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PJR
{
    public partial class ResourceSystem
    {
        public class YooAssetHandleWrapper : ResourceLoader
        {
            private AssetHandle assetHandle = null;

            private List<AssetHandle> assetHandles = new List<AssetHandle>();
            public YooAssetHandleWrapper(string assetFullName, Type assetType) : base(assetFullName, assetType)
            {
                assetHandle = ResourceSystem.instance.Package.LoadAssetAsync(Path.GetFileName(assetFullName), assetType);
            }
            public override void Update()
            {
                if (isDone)
                    return;

                if (!assetHandle.IsValid || !string.IsNullOrEmpty(assetHandle.LastError))
                {
                    error = $"[EditorResourceLoader] Find not asset \"{AssetName}\": \n" +
                        $" [type]: {AssetType.FullName} \n" +
                        $" [fullName]:{AssetFullName}  \n" +
                        $" [error]:{assetHandle.LastError}";
                    LogSystem.LogError(error);
                    State = LoaderState.Error;
                    return;
                }

                if (assetHandle.IsDone)
                    State = LoaderState.Done;
            }

            public override Object GetRawAsset()
            {
                return assetHandle.AssetObject;
            }
            public override T GetRawAsset<T>()
            {
                return assetHandle.AssetObject as T;
            }

            public override void Release()
            {
                if (assetHandle == null)
                    return;
                assetHandle.Release();
                State = LoaderState.Released;
            }
        }
        public class YooAssetHandleWrapper<T> : ResourceLoader where T : System.Type
        {
            public YooAssetHandleWrapper(T assetType, string assetFullName) : base(assetFullName, assetType)
            {
            }
        }
    }
}
