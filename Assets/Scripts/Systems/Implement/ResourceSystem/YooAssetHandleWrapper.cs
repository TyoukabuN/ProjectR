using System;
using System.Collections.Generic;
using System.IO;
using YooAsset;
using Object = UnityEngine.Object;

namespace PJR.Systems
{
    public partial class ResourceSystem
    {
        public class YooAssetHandleWrapper : ResourceLoader
        {
            private AssetHandle assetHandle = null;
            private SubAssetsHandle subAssetsHandle = null;

            private List<AssetHandle> assetHandles = new List<AssetHandle>();
            private YooAssetHandleWrapper(string assetName, Type assetType) : base(assetName, assetType)
            {
            }

            public static YooAssetHandleWrapper LoadAsset(string assetName, Type assetType)
            {
                assetName = Path.GetFileName(assetName);
                var handle = new YooAssetHandleWrapper(assetName, assetType);
                handle.assetHandle = instance.Package.LoadAssetAsync(Path.GetFileName(assetName), assetType);
                return handle;
            }
            public static YooAssetHandleWrapper LoadAssetByGuid(string assetGuid, Type assetType)
            {
                GetGuidInfo(assetGuid, out assetGuid, out _);
                AssetInfo assetInfo = YooAssets.GetAssetInfoByGUID(assetGuid, assetType);

                var handle = new YooAssetHandleWrapper(assetInfo.AssetPath, assetType);
                handle.assetHandle = instance.Package.LoadAssetAsync(assetInfo);
                return handle;
            }
            public static YooAssetHandleWrapper LoadSubAsset(string assetName, Type assetType)
            {
                var handle = new YooAssetHandleWrapper(assetName, assetType);
                handle.subAssetsHandle = instance.Package.LoadSubAssetsAsync(Path.GetFileName(assetName), assetType);
                return handle;
            }
            public static YooAssetHandleWrapper LoadSubAssetByGuid(string assetGuid, Type assetType)
            {
                GetGuidInfo(assetGuid, out assetGuid, out _);
                AssetInfo assetInfo = YooAssets.GetAssetInfoByGUID(assetGuid, assetType);

                var handle = new YooAssetHandleWrapper(assetInfo.AssetPath, assetType);
                handle.subAssetsHandle = instance.Package.LoadSubAssetsAsync(assetInfo);
                return handle;
            }
            

            public override void Update()
            {
                if (isDone)
                    return;

                if (!assetHandle.IsValid || !string.IsNullOrEmpty(assetHandle.LastError))
                {
                    error = $"[EditorResourceLoader] Find not asset \"{AssetName}\": \n" +
                        $" [type]: {AssetType.FullName} \n" +
                        $" [fullName]:{AssetName}  \n" +
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
        public class YooAssetHandleWrapper<T> : ResourceLoader where T : Type
        {
            public YooAssetHandleWrapper(T assetType, string assetName) : base(assetName, assetType)
            {
            }
        }
    }
}
