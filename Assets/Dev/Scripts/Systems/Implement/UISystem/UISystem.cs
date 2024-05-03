using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PJR
{
    public enum UILayer
    {
        Main = 0,
        Game,
        Top,
    }
    public class UISystem : MonoSingletonSystem<UISystem>
    {
        public GameObject UIRoot;
        //key : GameObjectInstanceID
        public Dictionary<UILayer, Dictionary<int, UINode>> nodeDict = new Dictionary<UILayer, Dictionary<int, UINode>>();
        public Dictionary<UILayer, Transform> rootDict = new Dictionary<UILayer,Transform>();
        public Transform CanvasRoot;

        public GameObject EventSystemObj;
        public override void Init()
        {
            UIRoot = this.gameObject;
            Type[] types = {typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler) };
            GameObject canvasObj = new GameObject("Canvas", types);
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.transform.SetParent(UIRoot.transform);
            CanvasRoot = canvasObj.transform;
            CanvasRoot.position = Vector3.zero;
            GenObject(UILayer.Main, "MainUIRoot");
            GenObject(UILayer.Game, "GameUIRoot");
            GenObject(UILayer.Top, "TopUIRoot");
            CreateEventSystem();
            Test();
            LogSystem.Log("========UI准备完成");
        }
        private void CreateEventSystem()
        {
            if (EventSystemObj==null)
            {
                Type[] types = { typeof(StandaloneInputModule), typeof(EventSystem) };
                GameObject obj = new GameObject("EventSystem", types);
                obj.transform.SetParent(UIRoot.transform);
            }
        }
        public void Test()
        {
            LoadUI("UI_Test_Panel.prefab");
        }
        private void GenObject(UILayer uilayer,string name)
        {
            GameObject obj = new GameObject(name,typeof(RectTransform));
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.position = Vector3.zero;
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            obj.transform.SetParent(CanvasRoot);
            rootDict[uilayer] = obj.transform;
        }
        public void ShowNormal(UINode node)
        {
            Array arr = Enum.GetValues(typeof(UILayer));
            
            for (int i = 0; i < arr.Length; i++)
            {
                if (node.layer != (UILayer)arr.GetValue(i))
                {
                    rootDict[node.layer].gameObject.SetActive(false);
                }
                else
                {
                    rootDict[node.layer].gameObject.SetActive(true);
                }
            }
            node.gameObject.SetActive(true);
            node.transform.SetAsLastSibling();
            node.OnOpen();
        }
        public void ShowTop(UINode node)
        {
            node.gameObject.SetActive(true);
            node.transform.SetAsLastSibling();
        }
        public void Close(UINode node,bool isRelease)
        {
            if (isRelease)
            {
                node.OnClose();
                node.OnDestory();
                nodeDict[node.layer].Remove(node.instanID);
                Destroy(node.gameObject);
            }
            else
            {
                node.OnClose();
                node.gameObject.SetActive(false);
            }
        }
        public void OpenPanel(string name,object obj =null)
        {
            if (true)
            {
                if(ResourceSystem.TryGetAsset(name, out ResourceLoader asset))
                {
                }
                
            }
        }
        public void LoadUI(string path)
        {
            StartCoroutine(LoadAsset(path));
        }
        IEnumerator LoadAsset(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var loader = ResourceSystem.LoadAsset<GameObject>(name);
                if (loader == null)
                {
                    LogSystem.LogError($"[{nameof(LoadAsset)}] Failure to load UI asset");
                    yield return null;
                }
                yield return loader;
                OnLoadDone(loader);
            }
        }
        public void OnLoadDone(ResourceLoader loader)
        {
            var asset = loader.GetRawAsset<GameObject>();
            if (asset == null)
            {
                LogSystem.LogError($"[{nameof(OnLoadDone)}] Failure to load UI asset");
                return;
            }

            GameObject ui = GameObject.Instantiate(asset);
            if (ui == null)
            {
                LogSystem.LogError($"[{nameof(OnLoadDone)}] Failure to instantiate UI");
                return;
            }
            UINode uinode = ui.TryGetComponent<UINode>();
            if (uinode!=null)
            {
                ui.transform.SetParent(rootDict[uinode.layer]);
                uinode.OnInit();
                if (nodeDict.ContainsKey(uinode.layer))
                {
                    nodeDict[uinode.layer][uinode.instanID] = uinode;
                }
                else
                {
                    nodeDict[uinode.layer] = new Dictionary<int, UINode>();
                    nodeDict[uinode.layer][uinode.instanID] = uinode;
                }
            }
        }
    }
}

