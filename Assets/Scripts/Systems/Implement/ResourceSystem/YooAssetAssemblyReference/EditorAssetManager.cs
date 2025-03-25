#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace YooAsset
{
    /// <summary>
    /// 一个Editor下全Asset的Cache,
    /// 为了在Asset没在Manifect的情况下也可以加载它
    /// </summary>
    public class EditorAssetManager
    {
        public string[] IgnoreExtension = new string[] {
            ".cs"
        };
        public string[] TargetFolder = new string[] {
            "Assets"
        };
        /// <summary>
        /// 这里key默认是带后缀的文件名
        /// </summary>
        private Dictionary<string, string> _location2guid = new Dictionary<string, string>();

        private Dictionary<string, PackageAsset> _assetPath2PackageAsset = new Dictionary<string, PackageAsset>();
        private Dictionary<string, PackageAsset> _assetName2PackageAsset = new Dictionary<string, PackageAsset>();

        private bool _hadInitCache = false;

        private string _packageName = string.Empty;

        public string PackageName => _packageName;

        public EditorAssetManager()
        {
            Initialize();
        }
        public EditorAssetManager(string packageName)
        {
            _packageName = packageName;
            Initialize();
        }
        private void Initialize()
        {
            if (_hadInitCache)
                return;
            RegisterAssetModifyEvent();
            CacheAllAsset();
            _hadInitCache = true;
        }
        private void RegisterAssetModifyEvent()
        {
            EditorAssetProcessor.OnWillCreateAssetCall -= OnWillCreateAsset;
            EditorAssetProcessor.OnWillCreateAssetCall += OnWillCreateAsset;
            EditorAssetProcessor.OnWillDeleteAssetCall -= OnWillDeleteAsset;
            EditorAssetProcessor.OnWillDeleteAssetCall += OnWillDeleteAsset;
            EditorAssetProcessor.OnWillMoveAssetCall -= OnWillMoveAsset;
            EditorAssetProcessor.OnWillMoveAssetCall += OnWillMoveAsset;
        }
        private void UnRegisterAssetModifyEvent()
        {
            EditorAssetProcessor.OnWillCreateAssetCall -= OnWillCreateAsset;
            EditorAssetProcessor.OnWillDeleteAssetCall -= OnWillDeleteAsset;
            EditorAssetProcessor.OnWillMoveAssetCall -= OnWillMoveAsset;
        }
        void OnWillCreateAsset(string assetName)
        {
            //这个时候是拿不到guid的
            Cache(assetName);
        }
        void OnWillDeleteAsset(string assetName)
        {
            UnCache(assetName);
        }
        void OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            UnCache(sourcePath);
            Cache(destinationPath);
        }
        public bool IsValidExtension(string assetPath)
        {
            var ext = Path.GetExtension(assetPath);
            return !IgnoreExtension.Contains(ext);
        }
        public void CacheAllAsset()
        {
            string[] guids = AssetDatabase.FindAssets(null, TargetFolder);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;
                Cache(assetPath);
            }
        }
        public void Cache(string assetPath)
        {
            if (!IsValidExtension(assetPath))
                return;
            if (!string.IsNullOrEmpty(assetPath))
            {
                string assetName = Path.GetFileName(assetPath);
                var packageAsset = new PackageAsset();
                packageAsset.Address = assetName;
                packageAsset.AssetPath = assetPath;
                packageAsset.AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                packageAsset.AssetTags = null;
                packageAsset.BundleID = -1;
                _assetPath2PackageAsset[assetPath] = packageAsset;
                _assetName2PackageAsset[assetName] = packageAsset;
            }
            //
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!string.IsNullOrEmpty(guid))
            { 
                string assetName = Path.GetFileName(assetPath);
                _location2guid[assetName] = guid;
            }
        }
        public void UnCache(string assetPath)
        {
            if (!IsValidExtension(assetPath))
                return;
            string assetName = Path.GetFileName(assetPath);
            if (!string.IsNullOrEmpty(assetPath))
            { 
                _assetPath2PackageAsset.Remove(assetPath);
                _assetName2PackageAsset.Remove(assetName);
            }
            //
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!string.IsNullOrEmpty(guid))
            { 
                _location2guid.Remove(assetName);
            }
        }
        public string ConvertLocationToAssetPath(string location)
        {
            if (_location2guid.TryGetValue(location, out string guid))
                return AssetDatabase.GUIDToAssetPath(guid);
            if (_assetName2PackageAsset.TryGetValue(location, out PackageAsset packageAsset1))
                return packageAsset1.AssetPath;
            if (_assetPath2PackageAsset.TryGetValue(location, out PackageAsset packageAsset2))
                return packageAsset2.AssetPath;
            return string.Empty;
        }
        private bool TryGetPackageAsset(string assetPath, out PackageAsset result)
        {
            return _assetPath2PackageAsset.TryGetValue(assetPath, out result);
        }
        /// <summary>
        /// 资源定位地址转换为资源信息。
        /// </summary>
        /// <returns>如果转换失败会返回一个无效的资源信息类</returns>
        public AssetInfo ConvertLocationToAssetInfo(string location, System.Type assetType)
        {
            string assetPath = ConvertLocationToAssetInfoMapping(location);
            if (TryGetPackageAsset(assetPath, out PackageAsset packageAsset))
            {
                AssetInfo assetInfo = new AssetInfo(PackageName, packageAsset, assetType);
                return assetInfo;
            }
            else
            {
                string error;
                if (string.IsNullOrEmpty(location))
                    error = $"The location is null or empty !";
                else
                    error = $"The location is invalid : {location}";
                AssetInfo assetInfo = new AssetInfo(PackageName, error);
                return assetInfo;
            }
        }
        /// <summary>
        /// 资源GUID转换为资源信息。
        /// </summary>
        /// <returns>如果转换失败会返回一个无效的资源信息类</returns>
        public AssetInfo ConvertAssetGUIDToAssetInfo(string assetGUID, System.Type assetType)
        {
            string assetPath = ConvertAssetGUIDToAssetInfoMapping(assetGUID);
            if (TryGetPackageAsset(assetPath, out PackageAsset packageAsset))
            {
                AssetInfo assetInfo = new AssetInfo(PackageName, packageAsset, assetType);
                return assetInfo;
            }
            else
            {
                string error;
                if (string.IsNullOrEmpty(assetGUID))
                    error = $"The assetGUID is null or empty !";
                else
                    error = $"The assetGUID is invalid : {assetGUID}";
                AssetInfo assetInfo = new AssetInfo(PackageName, error);
                return assetInfo;
            }
        }
        private string ConvertLocationToAssetInfoMapping(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                YooLogger.Error("Failed to mapping location to asset path, The location is null or empty.");
                return string.Empty;
            }

            string assetPath = ConvertLocationToAssetPath(location);
            if (!string.IsNullOrEmpty(assetPath))
            {
                return assetPath;
            }
            else
            {
                YooLogger.Warning($"Failed to mapping location to asset path : {location}");
                return string.Empty;
            }
        }
        private string ConvertAssetGUIDToAssetInfoMapping(string assetGUID)
        {
            if (string.IsNullOrEmpty(assetGUID))
            {
                YooLogger.Error("Failed to mapping assetGUID to asset path, The assetGUID is null or empty.");
                return string.Empty;
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            if (!string.IsNullOrEmpty(assetPath))
            {
                return assetPath;
            }
            else
            {
                YooLogger.Warning($"Failed to mapping assetGUID to asset path : {assetGUID}");
                return string.Empty;
            }
        }
        public void ClearAll()
        {
            _location2guid = null;
            _assetPath2PackageAsset = null;
        }
    }
}

#endif