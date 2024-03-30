using System;
using System.Collections.Generic;
using Hunter;
using System.IO;
using UnityEngine;
using UnityEditor.VersionControl;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LS.Game
{
    public class ConfigSystem //: MonoSystem<ConfigSystem>
    {
        //protected override string SystemName => "ConfigSystem";

        private static string ResourceLocalPath = "ConfigAssets";
        
        public static Dictionary<string, ConfigAsset> name2configAsset;
        
        // protected override void AwakeInternal()
        // {
            //LoadAllConfigAssets();
        //}

        public static string GetAssetPath(string assetName)
        {
            return Path.Combine(ResourceLocalPath, assetName);
        }
        /// <summary>
        /// 立即加载所有配置（Resource.LoadAll很耗时）
        /// </summary>
        private static void LoadAllConfigAssets()
        {
            name2configAsset ??= new Dictionary<string, ConfigAsset>();
            //TODO:[T]先用resource.load ,后面加assetsSystem用ab加载
            using (new Cost("LoadAllConfigAssets"))
            {
    #if UNITY_EDITOR
                //Resource.LoadAll的消耗很大
                var assets = Resources.LoadAll<ConfigAsset>(ResourceLocalPath);
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(assets[i].name);
                    name2configAsset[assets[i].name] = assets[i];
                }
    #endif
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private static ConfigAsset LoadConfigAssets(string assetName)
        {
            name2configAsset ??= new Dictionary<string, ConfigAsset>();
            
            using (new Cost("LoadConfigAssets"))
            {
                ConfigAsset asset = null;
                if (name2configAsset.TryGetValue(assetName, out asset))
                    return asset;

                string assetPath = GetAssetPath(assetName);

#if UNITY_EDITOR
                asset = Resources.Load<ConfigAsset>(assetPath);
                if (asset == null)
                    return asset;
#else
            //TODO:[T]后面要增加ResourceSystem之类的资源加载系统
#endif
                name2configAsset[assetName] = asset;
                return asset;
            }
        }

        private static bool TryGetAsset(string assetName, out ConfigAsset asset)
        {
            asset = LoadConfigAssets(assetName);
            return asset != null;
        }
        public static T LoadConfig<T>(string assetName) where T:ConfigAsset
        {
            T asset = null;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                if (!TryGetAsset(assetName, out ConfigAsset _asset))
                    return null;
                asset = (T)_asset;
#if UNITY_EDITOR
            }
            else
            {
                string assetPath = GetAssetPath(assetName);
                asset = Resources.Load<T>(assetPath);
            }
#endif
            return asset;
        }
    }

    public class Cost :System.IDisposable
    {
        public string name = String.Empty;
        private DateTime beginStamp;

        public Cost()
        {
            beginStamp = System.DateTime.Now;
        }
        public Cost(string name):this()
        {
            this.name = name;
        }

        public void Dispose()
        {
            if(string.IsNullOrEmpty(name))
                Debug.Log($"[Cost]: {(System.DateTime.Now - beginStamp).TotalMilliseconds} ms");
            else
                Debug.Log($"[Cost][{name}]: {(System.DateTime.Now - beginStamp).TotalMilliseconds} ms");
        }
    }
}