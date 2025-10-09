using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using YooAsset;
using Object = UnityEngine.Object;

namespace PJR.Systems
{
    public partial class ResourceSystem
    {
        #region AssetInfo获取相关

        public const string GuidPattern = @"(?<guid>^[\w]+)?(\[(?<subAsset>[^\[\]]+)\]$)?";
        /// <summary>
        /// 传进来的guid可能这样的：
        /// bf2889ed61cf07241a607a6deed04f55[153360_ECFG_Humanoid_GoblinArcher_10003]
        /// 来自<see cref="AssetReference.RuntimeKey"/>
        /// 或来自<see cref="AddressableExtensions.GetAddress"/>
        /// </summary>
        /// <param name="assetGUID"></param>
        /// <param name="guid"></param>
        /// <param name="subAssetName"></param>
        public static bool GetGuidInfo(string assetGuid, out string guid, out string subAssetName)
        {
            guid = assetGuid;
            subAssetName = null;
            Match match = new Regex(GuidPattern).Match(assetGuid);
            if (!match.Success)
                return false;
            guid = match.Groups["guid"].Value;
            subAssetName = match.Groups["subAsset"].Value;
            return true;
        }
        public static void GetGuidInfo(string assetGuid, out string guid)
            => GetGuidInfo(assetGuid, out guid, out _);
        public static string GetPureGuid(string assetGUID)
        {
            GetGuidInfo(assetGUID, out var guid, out _);
            return guid;
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetInfo"></param>
        /// <returns></returns>
        public static bool TryGetAssetInfoByName(string assetName, out AssetInfo assetInfo)
        {
            assetInfo = null;
            if (!IsValid())
                return false;
            assetInfo = YooAssets.GetAssetInfo(assetName, typeof(Object));
            return string.IsNullOrEmpty(assetInfo?.Error);
        }
        public static bool TryGetAssetInfoByGUID(string assetGUID, out AssetInfo assetInfo)
        {
            assetInfo = GetAssetInfoByGUID(assetGUID, typeof(Object));
            return string.IsNullOrEmpty(assetInfo?.Error);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="assetGUID">资源GUID</param>
        public static AssetInfo GetAssetInfoByGUID(string assetGUID)
        {
            if (!IsValid())
                return null;
            GetGuidInfo(assetGUID, out assetGUID, out var subAssetName);
            return YooAssets.GetAssetInfoByGUID(assetGUID);
        }
        public static AssetInfo GetAssetInfoByGUID(string assetGUID, Type type)
        {
            if (!IsValid())
                return null;
            GetGuidInfo(assetGUID, out assetGUID, out var subAssetName);
            return YooAssets.GetAssetInfoByGUID(assetGUID, type);
        }

        public static AssetInfo[] GetAssetInfos(string tag)
        {
            if (!IsValid())
                return null;
            return YooAssets.GetAssetInfos(tag);
        }

        public static AssetInfo[] GetAssetInfos(string[] tags)
        {
            if (!IsValid())
                return null;
            return YooAssets.GetAssetInfos(tags);
        }

        public static AssetInfo[] GetAssetInfos(List<string> tags)
        {
            if (!IsValid())
                return null;
            return YooAssets.GetAssetInfos(tags.ToArray());
        }

        #endregion

        #region 加载相关
        public static ResourceLoader LoadAsset<T>(string assetFullName) where T : Object
        {
            if (string.IsNullOrEmpty(assetFullName))
                return null;
            return LoadAsset<T>(assetFullName, null);
        }
        public static ResourceLoader LoadAsset<T>(string assetFullName, Action<ResourceLoader> callback) where T : Object
        {
            if (string.IsNullOrEmpty(assetFullName))
                return null;
            var loader = LoadAsset(assetFullName, typeof(T));
            if (loader.isDone)
                callback?.Invoke(loader);
            else if(callback != null)
                loader.Completed += callback;
            return loader;
        }
        public static ResourceLoader LoadAsset(string assetFullName, Type assetType)
        {
            if (string.IsNullOrEmpty(assetFullName))
                return null;

            string assetName = Path.GetFileName(assetFullName);

            if (!instance.assetFullName2Loader.TryGetValue(assetFullName, out var loader))
                instance.assetFullName2Loader.TryGetValue(assetFullName, out loader);

            if (loader == null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    loader = new EditorResourceLoader(assetFullName, assetType);
                }
                else
#endif
                {
#if UNITY_EDITOR
                    if (!InABMode)
                    {

                        loader = new EditorResourceLoader(assetFullName, assetType);
                    }
                    else
#endif
                    {
                        loader = YooAssetHandleWrapper.LoadAsset(assetFullName, assetType);
                    }
                }
                instance.assetFullName2Loader[assetFullName] = loader;
                instance.allLoaders.Add(loader);
                instance.assetName2Loader[assetName] = loader;
                instance.needUpdatingLoaders.Add(loader);
            }

            return loader;
        }

        /// <summary>
        /// 尝试用Asset文件名获取loader
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="loader"></param>
        /// <returns>loader != null并且loader.isDone返回true</returns>
        public static bool TryGetAsset(string assetName, out ResourceLoader loader)
        {
            if (instance.assetName2Loader.TryGetValue(assetName, out loader) && loader.isDone)
                return true;
            return false;
        }
        /// <summary>
        /// 尝试用Asset完整路径名字获取loader
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="loader"></param>
        /// <returns>loader != null并且loader.isDone返回true</returns>
        public static bool TryGetAssetByFullName(string assetNameWithExtension, out ResourceLoader loader)
        {
            if (instance.assetName2Loader.TryGetValue(assetNameWithExtension, out loader) && loader.isDone)
                return true;
            return false;
        }
        
        
        /// <summary>
        /// 根据AssetReference异步加载资源子对象
        /// </summary>
        /// <param name="assetReference">只用到里面的assetReference.AssetGUID</param>
        /// <returns></returns>
        public static SubAssetsHandle LoadSubAssetsByGuid(object obj, string assetGuid, Type type, out string subAssetName)
        {
            subAssetName = null;
            if (!IsValid())
                return null;
            if (string.IsNullOrEmpty(assetGuid))
                return null;

            GetGuidInfo(assetGuid, out assetGuid, out subAssetName);
            AssetInfo assetInfo = YooAssets.GetAssetInfoByGUID(assetGuid, type);
            
            // if (EditorMode)
            //     return LoadSubAssetWaitForComplete(assetInfo);

            var handle = YooAssets.LoadSubAssetsAsync(assetInfo);
            //CacheHandle(obj, handle, assetGuid);
            return handle;
        }
        public static SubAssetsHandle LoadSubAssetsByGuid(object obj, string assetGuid, Type type)
            => LoadSubAssetsByGuid(obj, assetGuid, type, out _);
        public static SubAssetsHandle LoadSubAssets(object obj, AssetReference assetReference, Type type, out string subAssetName)
            => LoadSubAssetsByGuid(obj, assetReference.AssetGUID, type, out subAssetName);
        public static SubAssetsHandle LoadSubAssets(object obj, AssetReference assetReference, Type type)
            => LoadSubAssets(obj, assetReference, type, out _);

        #endregion

    }
}