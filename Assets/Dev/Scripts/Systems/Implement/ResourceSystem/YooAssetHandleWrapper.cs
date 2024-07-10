using System;
using UnityEditor;
using UnityEngine;
using YooAsset;
using System.IO;
using System.Collections.Generic;

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
                assetHandle = ResourceSystem.inst.Package.LoadAssetAsync(Path.GetFileName(assetFullName), assetType);
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

            public override object GetRawAsset()
            {
                return assetHandle.AssetObject;
            }
            public override T GetRawAsset<T>()
            {
                return assetHandle.AssetObject as T;
            }

            public override void Release()
            {
                base.Release();
                if (assetHandle == null)
                    return;
                assetHandle.Release();
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
