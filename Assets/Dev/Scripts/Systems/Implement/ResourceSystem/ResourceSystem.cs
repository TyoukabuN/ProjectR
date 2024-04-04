using PJR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

namespace PJR
{
    public class ResourceSystem : MonoSingletonSystem<ResourceSystem>
    {
        public Dictionary<string,ResourceLoader> assetFullName2Loader = new Dictionary<string, ResourceLoader> ();
        public ResourceLoader LoadAsset<T>(string assetFullName) where T : System.Type
        {
            return LoadAsset(assetFullName, typeof(T));
        }
        public ResourceLoader LoadAsset(string assetFullName,Type assetType) 
        {
#if UNITY_EDITOR
            var loader = new EditorResourceLoader(assetFullName, assetType);
            assetFullName2Loader.Add(assetFullName, loader);
            return loader;
#endif
            //TODO:LoadAsset
            return null;
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
                            loader.OnDone(loader);
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