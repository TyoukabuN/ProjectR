using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using YooAsset;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public partial class ResourceSystem : MonoSingletonSystem<ResourceSystem>
    {
        public static Dictionary<string,ResourceLoader> assetFullName2Loader = new Dictionary<string, ResourceLoader> ();

        public static Dictionary<string,ResourceLoader> assetame2Loader = new Dictionary<string, ResourceLoader> ();

        private ResourcePackage _package;
        public ResourcePackage Package { get { return _package; } }

#if UNITY_EDITOR
        private static EditorAssetManager _editorAssetMgr;
#endif
        private static ResourceDescription _resDesc = null;
        //
        public static ResourceDescription ResDesc => _resDesc;
        public static bool IsRequireUpdate => _resDesc != null && _resDesc.RequireUpdate == true;

        public const string PackageName = "Default";

        /// <summary>
        /// 主动更新的主线程阻塞时长上限
        /// 默认5秒
        /// </summary>
        public static int InitiativeUpdateLoopLimit = 5000;
        private static Stopwatch _watch;
        private static Stopwatch Watch
        {
            get
            {
                _watch = _watch ?? new Stopwatch();
                return _watch;
            }
        }
        public static bool InABMode
        {
            get
            {
                bool res = true;
#if UNITY_EDITOR
                res = DebugMenu.InABMode;
#endif
                return Application.isPlaying && res;
            }
        }
#if UNITY_EDITOR
        public static EditorAssetManager EditorAssetMgr
        {
            get
            {
                if (_editorAssetMgr == null)
                    _editorAssetMgr = new EditorAssetManager();
                return _editorAssetMgr;
            }
        }
#endif
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Initialize()
        {
            LoadResDesc();
            yield return InitializePackage();
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <returns></returns>
        public IEnumerator UpdateResource()
        {
            yield return UpdatePackageAsync();
        }

        /// <summary>
        /// 加载资源描述
        /// </summary>
        /// <returns></returns>
        private static bool LoadResDesc()
        {
            string filePath = PathUtility.GetDescFilePath();
            if (!File.Exists(filePath))
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    ResourceDescription.CreateDesc();
                }
                else
#endif
                { 
                    LogSystem.LogError($"find not ResourceDescription in {filePath}");
                    return false;
                }
            }
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    try
                    {
                        _resDesc = JsonUtility.FromJson<ResourceDescription>(reader.ReadToEnd());
                    }
                    catch (Exception e)
                    {
                        LogSystem.LogError(e);
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 异步初始化包
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializePackage()
        {
            LogSystem.Log($"[AssetSystem.InitializePackage] 初始化资源包");

            YooAssets.Destroy();
            YooAssets.Initialize();

            string packageName = PackageName;
            InitializationOperation initOperation = null;

            Watch.Restart();
            
            {
                ResourcePackage package = YooAssets.TryGetPackage(packageName);
                if (package != null)
                {
                    LogSystem.LogError($"已存在 {packageName}");
                    yield return null;
                }

                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);

                if (!InABMode)
                {
                    var initParameters = new EditorSimulateModeParameters();
                    initParameters.SimulateManifestFilePath = GetStreamingAssetsRoot();
                    if (!Directory.Exists(initParameters.SimulateManifestFilePath))
                        yield break;
                    var filePaths = Directory.GetFiles(initParameters.SimulateManifestFilePath, "*.bytes", SearchOption.AllDirectories);
                    if (filePaths.Length <= 0)
                    {
                        yield break;
                    }
                    initParameters.SimulateManifestFilePath = filePaths[0];
                    initOperation = package.InitializeAsync(initParameters);
                    yield return initOperation;
                }
                else
                {
                    if (IsRequireUpdate)
                    {
                        //默认描述里有默认跟备用资源地址
                        string defaultHostServer = _resDesc.RemoteUrls[0];
                        string fallbackHostServer = _resDesc.RemoteUrls[1];
                        string channel = _resDesc.Channel;
                        string platform = _resDesc.Platform;

                        var initParameters = new HostPlayModeParameters();
                        initParameters.BuildinQueryServices = new GameQueryServices();
                        initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer, channel, platform, packageName);
                        initOperation = package.InitializeAsync(initParameters);
                        yield return initOperation;
                    }
                    else
                    {
                        var initParameters = new OfflinePlayModeParameters();
                        initOperation = package.InitializeAsync(initParameters);
                        yield return initOperation;
                    }
                }

                if (initOperation.Status != EOperationStatus.Succeed)
                {
                    LogSystem.LogError($"[AssetSystem.InitializePackage] 资源包初始化失败：{packageName}");
                    yield break;
                }

                Watch.Stop();
                LogSystem.Log($"[AssetSystem.InitializePackage] 资源包初始化成功：{packageName}  耗时：{Watch.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// 更新Package
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="appendTimeTicks"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEnumerator UpdatePackageAsync(string packageName = null, bool appendTimeTicks = false, int timeout = 60)
        {
            Watch.Restart();
            if (string.IsNullOrEmpty(packageName))
                packageName = PackageName;
            if (!InABMode)
                yield return null;
            //需要更新？
            if (!IsRequireUpdate)
                yield return null;

            LogSystem.Log($"[AssetSystem.UpdatePackage] 更新Package [{packageName}]");

            var package = YooAssets.GetPackage(packageName);
            var updatePackageVersionOperation = package.UpdatePackageVersionAsync(appendTimeTicks, timeout);
            yield return updatePackageVersionOperation;

            if (updatePackageVersionOperation.Status != EOperationStatus.Succeed)
            {
                LogSystem.LogError($"[AssetSystem.UpdatePackage] 更新版本号失败：{packageName}");
                yield return null;
            }
            //
            LogSystem.Log($"[AssetSystem.UpdatePackage] 更新版本号成功：{packageName}");
            var packageVersion = updatePackageVersionOperation.PackageVersion;
            var savePackageVersion = true;
            var updatePackageManifestOperation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion, timeout);
            yield return updatePackageManifestOperation;

            if (updatePackageManifestOperation.Status != EOperationStatus.Succeed)
            {
                LogSystem.LogError($"[AssetSystem.UpdatePackage] 更新资源清单失败：{packageName}");
                yield return null;
            }

            Watch.Stop();
            LogSystem.Log($"[AssetSystem.UpdatePackage] 更新资源清单成功：{packageName}  耗时：{Watch.ElapsedMilliseconds}ms");
        }


        public static ResourceLoader LoadAsset<T>(string assetFullName) where T : UnityEngine.Object 
        {
            if(string.IsNullOrEmpty(assetFullName))
                return null;
            return LoadAsset(assetFullName, typeof(T));
        }
        public static ResourceLoader LoadAsset(string assetFullName,Type assetType) 
        {
            if (string.IsNullOrEmpty(assetFullName))
                return null;

            string assetName = Path.GetFileName(assetFullName);

            if (!assetame2Loader.TryGetValue(assetFullName, out var loader))
            {
                assetFullName2Loader.TryGetValue(assetFullName, out loader);
            }
            

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
                        loader = new YooAssetHandleWrapper(assetFullName, assetType);
                    }
                }
                assetFullName2Loader[assetFullName] = loader;
                assetame2Loader[assetName] = loader; 
            }

            return loader;
        }

        /// <summary>
        /// 尝试用名字获取loader
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="loader"></param>
        /// <returns>loader != null并且loader.isDone返回true</returns>
        public static bool TryGetAsset(string assetName,out ResourceLoader loader)
        {
            if (assetame2Loader.TryGetValue(assetName, out loader) && loader.isDone)
                return true;
            return false;
        }

        private List<string> dones = new List<string> ();   
        void Update()
        {
            if (assetFullName2Loader.Count <= 0)
                return;
            UpdateAllLoader(ref dones);
            ClearDones();
        }

        private void ClearDones()
        {
            if (dones.Count > 0)
            {
                foreach (var assetFullName in dones)
                {
                    if (assetFullName2Loader.TryGetValue(assetFullName, out var loader))
                    {
                        if (loader.OnDone != null)
                        {
                            try { 
                                loader.OnDone.Invoke(loader);
                                loader.OnDone = null;
                            }
                            catch (Exception e)
                            {
                                LogSystem.LogError($"[{nameof(ResourceSystem.ClearDones)}] {e.ToString()}");
                            }
                            loader.OnDone = null;
                        };

                        assetFullName2Loader.Remove(assetFullName);
                    }
                }
                dones.Clear();
            }
        }

        public void UpdateAllLoader(ref List<string> dones)
        {
            for (int i = 0; i < assetFullName2Loader.Count; i++)
            {
                var pair = assetFullName2Loader.ElementAt(i);
                var loader = pair.Value;
                loader.Update();

                if (loader.isDone)
                {
                    dones.Add(pair.Key);
                }
            }
        }

        /// <summary>
        /// 获取流文件夹路径
        /// </summary>
        public static string GetStreamingAssetsRoot()
        {
            return $"{Application.dataPath}/StreamingAssets/res/";
        }


    }
}