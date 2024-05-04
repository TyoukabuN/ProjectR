using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PJR
{
    public class UIModel : MonoBehaviour
    {
        public string prefabName;
        public UnityAction onLoadDone;
        public Vector3 offset;
        public Vector3 scale;
        public Vector3 rotation;
        private void Start()
        {
            Load(prefabName);
        }
        public void Load(string prefabName)
        {
            StartCoroutine(LoadAsset(prefabName));
        }
        IEnumerator LoadAsset(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var loader = ResourceSystem.LoadAsset<GameObject>(name);
                if (loader == null)
                {
                    LogSystem.LogError($"[{nameof(LoadAsset)}] Failure to load UIModel");
                    yield return null;
                }
                yield return loader;
                OnLoadDone(loader);
            }
        }
        private void OnLoadDone(ResourceLoader loader)
        {
            var asset = loader.GetRawAsset<GameObject>();
            if (asset == null)
            {
                LogSystem.LogError($"[{nameof(OnLoadDone)}] Failure to load LoadAsset");
                return;
            }
            GameObject model = GameObject.Instantiate(asset);
            model.transform.SetParent(transform);
            model.transform.localPosition = Vector3.zero + offset;
            model.transform.localScale = scale;
            model.transform.eulerAngles = rotation;
            for (int i = 0; i < transform.childCount; i++)
            {
                model.transform.GetChild(i).gameObject.layer = 5;
            }
            if (onLoadDone!=null)
            {
                onLoadDone();
            }
        }
    }
}

