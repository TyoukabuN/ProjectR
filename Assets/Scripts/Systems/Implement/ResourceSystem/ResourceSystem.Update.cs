using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Systems
{
    public partial class ResourceSystem
    {
        private List<string> dones = new List<string>();
        public override void OnUpdate(float deltaTime)
        {
            if (assetFullName2Loader.Count <= 0)
                return;
            UpdateAllLoader(ref dones);
            //ClearDones();
            ReleaseNonReferedLoader();
        }
        public void UpdateAllLoader(ref List<string> dones)
        {
            if (needUpdatingLoaders.Count > 0)
            {
                updatingLoaders.AddRange(needUpdatingLoaders);
                needUpdatingLoaders.Clear();
            }

            for (int i = 0; i < updatingLoaders.Count; i++)
            {
                var loader = updatingLoaders[i];
                if (loader.isDone)
                    continue;
                loader.Update();

                if (loader.isDone)
                {
                    try
                    {
                        loader.Completed?.Invoke(loader);
                        loader.Completed = null;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[{nameof(ResourceSystem.UpdateAllLoader)}] {e.ToString()}");
                    }
                    loader.Completed = null;
                }
            }
        }

        /// <summary>
        /// 多少帧清理一次没有被引用的Loader
        /// </summary>
        public int NonReferedLoaderReleaseFrameInterval = 5;

        /// <summary>
        /// 清理一次没有被引用的Loader
        /// </summary>
        public void ReleaseNonReferedLoader()
        {
            if (Time.frameCount % NonReferedLoaderReleaseFrameInterval != 0)
                return;
            for (int i = 0; i < allLoaders.Count; i++)
            {
                var loader = allLoaders[i];

                loader.ClearNullInstantiate();

                if (loader.Releasable())
                    loader.Release();

                if (loader.isReleased)
                    RemoveLoaderAt(i--);
            }
        }

        /// <summary>
        /// 移除Index下Loader
        /// </summary>
        /// <param name="index"></param>
        private void RemoveLoaderAt(int index)
        {
            if (index < 0 || index >= allLoaders.Count)
                return;
            var loader = allLoaders[index];
            allLoaders.RemoveAt(index);
            RemoveLoader(loader, true);
        }

        /// <summary>
        /// 移除Loader
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="ignoreAllList">从RemoveLoaderAt来的可以忽略掉allLoaders</param>
        private void RemoveLoader(ResourceLoader loader, bool ignoreAllLoaders = false)
        {
            if (loader == null)
                return;
            if(!ignoreAllLoaders) 
                allLoaders.Remove(loader);
            assetFullName2Loader.Remove(loader.AssetPath);
            assetName2Loader.Remove(loader.AssetName);
            updatingLoaders.Remove(loader);
            needUpdatingLoaders.Remove(loader);
        }
    }
}


