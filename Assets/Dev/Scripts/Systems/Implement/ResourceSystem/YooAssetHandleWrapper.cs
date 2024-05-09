using System;
using UnityEditor;
using UnityEngine;
using YooAsset;
using System.IO;

namespace PJR
{
    public class YooAssetHandleWrapper : ResourceLoader
    {
        public AssetHandle assetHandle = null;
        public YooAssetHandleWrapper(string assetFullName, Type assetType) : base(assetFullName, assetType)
        {
            assetHandle = ResourceSystem.inst.Package.LoadAssetSync(Path.GetFileNameWithoutExtension(assetFullName), assetType);
        }
        public override void Update()
        {
            if (isDone)
                return;

            if (!assetHandle.IsValid || !string.IsNullOrEmpty(assetHandle.LastError))
            {
                error = $"[EditorResourceLoader] Find not asset \"{assetName}\": \n" +
                    $" [type]: {assetType.FullName} \n" +
                    $" [fullName]:{assetFullName}  \n" +
                    $" [error]:{assetHandle.LastError}";
                LogSystem.LogError(error);
                phase = Phase.Done;
                return;
            }

            if (assetHandle.IsDone)
                phase = Phase.Done;
        }

        public override object GetRawAsset()
        {
            return assetHandle.AssetObject;
        }
        public override T GetRawAsset<T>()
        {
            return assetHandle.AssetObject as T;
        }
    }
    public class YooAssetHandleWrapper<T> : ResourceLoader where T : System.Type
    {
        public YooAssetHandleWrapper(T assetType, string assetFullName) : base(assetFullName, assetType)
        {
        }
    }
}
