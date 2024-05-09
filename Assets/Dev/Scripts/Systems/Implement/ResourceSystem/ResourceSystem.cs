using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using YooAsset;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public class ResourceSystem : MonoSingletonSystem<ResourceSystem>
    {
        public static Dictionary<string,ResourceLoader> assetFullName2Loader = new Dictionary<string, ResourceLoader> ();

        public static Dictionary<string,ResourceLoader> assetame2Loader = new Dictionary<string, ResourceLoader> ();

        private ResourcePackage _package;
        public ResourcePackage Package { get { return _package; } }

        public override IEnumerator Initialize()
        {
            if(!YooAssets.Initialized)
                YooAssets.Initialize();
            if (_package == null)
            { 
                _package = YooAssets.CreatePackage("pkgs");
                YooAssets.SetDefaultPackage(_package);
            }

            var initParameters = new OfflinePlayModeParameters();
            yield return _package.InitializeAsync(initParameters);
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
#if UNITY_EDITOR


            if (!assetame2Loader.TryGetValue(assetFullName, out var loader))
            {
                assetFullName2Loader.TryGetValue(assetFullName, out loader);
            }

            if (loader == null)
            {
                if (EditorPrefs.GetBool(DebugMenu.PJR_DebugMenuKey_LaunchInAssetBundleMode))
                {
                    
                    loader = new YooAssetHandleWrapper(assetFullName, assetType);
                }
                else
                { 
                    loader = new EditorResourceLoader(assetFullName, assetType);
                }
                assetFullName2Loader[assetFullName] = loader;
                assetame2Loader[assetName] = loader; 
            }

            return loader;
#else
            //TODO:LoadAsset
            return null;
#endif
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
    }
}