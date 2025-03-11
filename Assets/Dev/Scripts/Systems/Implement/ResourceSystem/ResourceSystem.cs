using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using YooAsset;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Systems
{
    public partial class ResourceSystem : MonoSingletonSystem<ResourceSystem>
    {
        public const int Default_List_Capacity = 256;
        public Dictionary<string,ResourceLoader> assetFullName2Loader = new Dictionary<string, ResourceLoader> ();

        public Dictionary<string,ResourceLoader> assetName2Loader = new Dictionary<string, ResourceLoader> ();

        public List<ResourceLoader> allLoaders = new List<ResourceLoader>(Default_List_Capacity);

        /// <summary>
        /// 正在更新的Loader
        /// </summary>
        public List<ResourceLoader> updatingLoaders = new List<ResourceLoader>(Default_List_Capacity);
        /// <summary>
        /// 需要更新的
        /// 防止在更新过程中产生Loader的对本次更新造成影响
        /// </summary>
        public List<ResourceLoader> needUpdatingLoaders = new List<ResourceLoader>(Default_List_Capacity);


        private ResourcePackage package;
        public ResourcePackage Package { get { return package; } }

        private ResourceDescription _resDesc = null;
        //
        public ResourceDescription ResDesc => _resDesc;
        public bool IsRequireUpdate => _resDesc != null && _resDesc.RequireUpdate == true;

        public const string PackageName = "Default";

        protected bool _initialized = false;
        public bool Initialized => _initialized;

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

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Initialize()
        {
            LoadResDesc();
            yield return InitializePackage();
        }

        public async UniTask InitializeAsync()
        {
            if (_initialized)
                return;
            _initialized = false;
            LoadResDesc();
            await InitializePackage();
            await UpdatePackageAsync(PackageName);
        }
        public void InitializeSync(bool force = false)
        {
            if (_initialized && !force)
                return;
            _initialized = false;
            LoadResDesc();
            InitializePackageSync();
            UpdatePackageSync();
            Watch.Reset();
        }

        /// <summary>
        /// 加载资源描述
        /// </summary>
        /// <returns></returns>
        private static bool LoadResDesc()
        {
            string filePath = PathUtil.GetDescFilePath();
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
                    Debug.LogError($"find not ResourceDescription in {filePath}");
                    return false;
                }
            }
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    try
                    {
                        instance._resDesc = JsonUtility.FromJson<ResourceDescription>(reader.ReadToEnd());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 同步加载本地AB
        /// 默认15秒
        /// </summary>
        private const long SyncInitializeBreakMilliseconds = 15000;
        /// <summary>
        /// 同步初始化包
        /// </summary>
        /// <returns></returns>
        private void InitializePackageSync()
        {
            Debug.Log($"[AssetSystem.InitializePackageSync] 初始化资源包");

            YooAssets.Destroy();
            YooAssets.Initialize();

            string packageName = PackageName;
            InitializationOperation initOperation = null;

            Watch.Restart();
            //Default (工程里可能有一个包，因为没有将Art分出来)
            while (true)
            {
                if (Watch.ElapsedMilliseconds > SyncInitializeBreakMilliseconds)
                    break;
                if (initOperation != null)
                {
                    if (initOperation.Status == EOperationStatus.Failed)
                    {
                        Debug.LogError($"[AssetSystem.InitializePackageSync] 资源包初始化失败：{packageName}");
                    }
                    else if (initOperation.Status == EOperationStatus.Processing)
                    {
                        YooAsset.OperationSystem.Update(true);
                    }
                    else if (initOperation.Status == EOperationStatus.Succeed)
                    {
                        Watch.Stop();
                        Debug.Log($"[AssetSystem.InitializePackageSync] 资源包初始化成功：{packageName}  耗时：{Watch.ElapsedMilliseconds}ms");
                        _initialized = true;
                        break;
                    }
                }
                else
                {
                    packageName = "Default";
                    package = YooAssets.TryGetPackage(packageName);
                    if (package != null)
                    {
                        Debug.LogWarning($"已存在 {packageName}");
                    }

                    package = YooAssets.CreatePackage(packageName);
                    YooAssets.SetDefaultPackage(package);


                    if (!InABMode)
                    {
                        var initParameters = new EditorSimulateModeParameters();
                        initParameters.SimulateManifestFilePath = GetStreamingAssetsRoot();
                        var filePaths = Directory.GetFiles(initParameters.SimulateManifestFilePath, "*.bytes", SearchOption.AllDirectories);
                        if (filePaths.Length <= 0)
                        {
                            break;
                        }
                        initParameters.SimulateManifestFilePath = filePaths[0];
                        initOperation = package.InitializeAsync(initParameters);
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
                        }
                        else
                        {
                            var initParameters = new OfflinePlayModeParameters();
                            initOperation = package.InitializeAsync(initParameters);
                        }
                    }
                }
            }
            if (_initialized == false)
            {
                Debug.LogError($"[AssetSystem.InitializePackageSync] 资源包初始化失败：{packageName}");
            }
        }

        /// <summary>
        /// 异步初始化包
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializePackage()
        {
            Debug.Log($"[ResourceSystem.InitializePackage] 初始化资源包");

            YooAssets.Destroy();
            YooAssets.Initialize();

            string packageName = PackageName;
            InitializationOperation initOperation = null;

            Watch.Restart();
            
            {
                ResourcePackage package = YooAssets.TryGetPackage(packageName);
                if (package != null)
                {
                    Debug.LogError($"已存在 {packageName}");
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
                    Debug.LogError($"[ResourceSystem.InitializePackage] 资源包初始化失败：{packageName}");
                    yield break;
                }

                Watch.Stop();
                Debug.Log($"[ResourceSystem.InitializePackage] 资源包初始化成功：{packageName}  耗时：{Watch.ElapsedMilliseconds}ms");
            }
        }



        /// <summary>
        /// 同步更新版本
        /// 默认15秒
        /// </summary>
        private const long SyncUpdatePackageBreakMilliseconds = 15000;
        public void UpdatePackageSync(string packageName = null, bool appendTimeTicks = false, int timeout = 60)
        {
            if (string.IsNullOrEmpty(packageName))
                packageName = PackageName;
            if (!InABMode)
                return;
            //需要更新？
            if (!IsRequireUpdate)
                return;
            var package = YooAssets.GetPackage(packageName);
            if (package == null)
                return;

            Watch.Restart();
            //同步更新版本好
            UpdatePackageVersionOperation updatePackageVersionOperation = null;
            while (true)
            {
                if (Watch.ElapsedMilliseconds > SyncUpdatePackageBreakMilliseconds)
                {
                    Debug.LogError($"[AssetSystem.UpdatePackage] 同步更新版本号失败：{packageName}");
                    return;
                }
                if (updatePackageVersionOperation == null)
                {
                    Debug.Log($"[AssetSystem.UpdatePackageSync] 同步更新Package [{packageName}]");
                    updatePackageVersionOperation = package.UpdatePackageVersionAsync(appendTimeTicks, timeout);
                }
                else
                {
                    if (updatePackageVersionOperation.Status == EOperationStatus.Failed)
                    {
                        Debug.LogError($"[AssetSystem.UpdatePackageSync] 同步更新版本号失败：{packageName}");
                        return;
                    }
                    else if (updatePackageVersionOperation.Status == EOperationStatus.Processing)
                    {
                        YooAsset.OperationSystem.Update(true);
                    }
                    else if (updatePackageVersionOperation.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log($"[AssetSystem.UpdatePackageSync] 同步更新版本号成功：{packageName} {updatePackageVersionOperation.PackageVersion}  耗时：{Watch.ElapsedMilliseconds}ms");
                        break;
                    }
                }
            }
            //同步更新版本号
            Watch.Restart();
            var packageVersion = updatePackageVersionOperation.PackageVersion;
            var savePackageVersion = true;

            UpdatePackageManifestOperation updatePackageManifestOperation = null;
            while (true)
            {
                if (Watch.ElapsedMilliseconds > SyncUpdatePackageBreakMilliseconds)
                {
                    Debug.LogError($"[AssetSystem.UpdatePackageSync] 同步更新资源清单失败：{packageName}");
                    return;
                }
                if (updatePackageManifestOperation == null)
                {
                    updatePackageManifestOperation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion, timeout);
                }
                else
                {
                    if (updatePackageManifestOperation.Status == EOperationStatus.Failed)
                    {
                        Debug.LogError($"[AssetSystem.UpdatePackageSync] 同步更新资源清单失败：{packageName}");
                        return;
                    }
                    else if (updatePackageManifestOperation.Status == EOperationStatus.Processing)
                    {
                        YooAsset.OperationSystem.Update(true);
                    }
                    else if (updatePackageManifestOperation.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log($"[AssetSystem.UpdatePackageSync] 同步更新资源清单成功：{packageName}  耗时：{Watch.ElapsedMilliseconds}ms");
                        break;
                    }
                }
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

            Debug.Log($"[ResourceSystem.UpdatePackage] 更新Package [{packageName}]");

            var package = YooAssets.GetPackage(packageName);
            var updatePackageVersionOperation = package.UpdatePackageVersionAsync(appendTimeTicks, timeout);
            yield return updatePackageVersionOperation;

            if (updatePackageVersionOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"[ResourceSystem.UpdatePackage] 更新版本号失败：{packageName}");
                yield return null;
            }
            //
            Debug.Log($"[ResourceSystem.UpdatePackage] 更新版本号成功：{packageName}");
            var packageVersion = updatePackageVersionOperation.PackageVersion;
            var savePackageVersion = true;
            var updatePackageManifestOperation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion, timeout);
            yield return updatePackageManifestOperation;

            if (updatePackageManifestOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"[ResourceSystem.UpdatePackage] 更新资源清单失败：{packageName}");
                yield return null;
            }

            Watch.Stop();
            Debug.Log($"[ResourceSystem.UpdatePackage] 更新资源清单成功：{packageName}  耗时：{Watch.ElapsedMilliseconds}ms");
        }


        /// <summary>
        /// 获取流文件夹路径
        /// </summary>
        public static string GetStreamingAssetsRoot()
        {
            return $"{Application.dataPath}/StreamingAssets/res/";
        }

        public static bool IsValid()
        {

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                ResourceSystem sys = instance;
                if (sys == null)
                    return false;
                if (instance.package == null)
                {
                    YooAssets.Destroy();
                    //sys.InitializeSync(true);
                }
                else
                    YooAssets.SetDefaultPackage(instance.package);
            }
            else
#endif
            {
                if (instance == null)
                    return false;
            }

            //if (!instance._initialized)
            //    return false;
            return true;
        }
    }
}