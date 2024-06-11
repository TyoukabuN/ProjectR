using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using Sirenix.OdinInspector;
using System.Xml.Schema;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;
using PJR;






#if UNITY_EDITOR
using UnityEditor;
#endif

public class ABLoadTest : MonoBehaviour
{
    public string resUrl = "https://192.168.10.50/ls/StandaloneWindows64/Art";
    public string assetName = string.Empty;
    public ResourcePackage package = null;

    public GameObject gobj = null;

    [Button("Destroy")]
    public void Destroy()
    {
        package = null;
        YooAssets.Destroy();
    }

    [Button("InitializePackage")]
    public void InitializePackage()
    {
        package = null;
        StartCoroutine(EInitializePackage());
    }

    [Button("LoadAsset")]
    public void LoadAsset()
    {
        if (package == null)
            return;
        var assetHandle = YooAssets.LoadAssetAsync<GameObject>(assetName);
        assetHandle.Completed -= OnComplete;
        assetHandle.Completed += OnComplete;
    }

    [Button()]
    public void RegisterEvent()
    {
        //if (!CMDUtility.IsBatchMode)
        //{ 
        Application.logMessageReceived -= LogCallback;
        Application.logMessageReceived += LogCallback;
        //}
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
            LogSystem.Log(stackTrace);
        else if (type == LogType.Warning)
            LogSystem.LogWarning(stackTrace);
        else
            LogSystem.LogError(stackTrace);
    }

    [Button()]
    public void Debug1()
    {
        Debug.Log("123");
        Debug.LogError("123");
    }

    public void OnComplete(AssetHandle assetHandle)
    {
        if (assetHandle != null)
        {
            gobj = assetHandle.InstantiateSync();
            Debug.Log(gobj?.name);
        }
    }

    public IEnumerator EInitializePackage()
    {
        YooAssets.Initialize();
        var packageName = "Art";
        package = YooAssets.TryGetPackage(packageName);
        if (package != null)
        {
            Debug.Log($"已存在{packageName}");
            yield return null;
        }

        package = YooAssets.CreatePackage("Art");
        YooAssets.SetDefaultPackage(package);

        string url = resUrl;
        var initParameters = new HostPlayModeParameters();
        initParameters.BuildinQueryServices = new GameQueryServices();
        initParameters.RemoteServices = new RemoteServices(url, url);
        var initOperation = package.InitializeAsync(initParameters);
        yield return initOperation;

        if (initOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功！");
        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}

