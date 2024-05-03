using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        //根节点字典
        public Dictionary<UILayer, Transform> rootDict = new Dictionary<UILayer,Transform>();
        //按名字储存的实例
        public Dictionary<string,GameObject> nameObjListDict = new Dictionary<string,GameObject>();
        public Transform CanvasRoot;

        public GameObject EventSystemObj;

        public UIAssetDict UIAssetdict;
        public override void Init()
        {
            UIAssetdict = UIAssetDataFunc.GetBindUIAssetDict();
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
            TestEntrance();
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
        public void TestEntrance()
        {
            OpenPanel("mainUI");
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
                if (node.layer == (UILayer)arr.GetValue(i))
                {
                    rootDict[node.layer].gameObject.SetActive(true);
                }
                else
                {
                    rootDict[(UILayer)arr.GetValue(i)].gameObject.SetActive(false);
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
                nameObjListDict.Remove(node.UIName);
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
            if (UIAssetdict.assets.ContainsKey(name))
            {
                if (!nameObjListDict.ContainsKey(name))
                {
                    LoadUI(UIAssetdict.assets[name].prefab,true);
                }
                else
                {
                    ShowNormal(nameObjListDict[name].GetComponent<UINode>());
                }
            }
            else
            {
                LogSystem.LogError($"不存在名称{name}的UIPanel");
            }
        }
        private void LoadUI(string path,bool isDoneShow = false)
        {
            StartCoroutine(LoadAsset(path, isDoneShow));
        }
        IEnumerator LoadAsset(string name, bool isDoneShow)
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
                OnLoadDone(loader, isDoneShow);
            }
        }
        public void OnLoadDone(ResourceLoader loader,bool isDoneShow)
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
                nameObjListDict[uinode.UIName] = ui;
                //重设rect
                ui.transform.SetParent(rootDict[uinode.layer]);
                RectTransform rect = ui.TryGetComponent<RectTransform>();
                rect.position = Vector3.zero;
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

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
                if (isDoneShow)
                {
                    ShowNormal(nameObjListDict[uinode.UIName].GetComponent<UINode>());
                }
            }
        }
    }
    public static class UIAssetDataFunc
    {
        public static UIAssetDict GetBindUIAssetDict()
        {
            string writePath = Application.dataPath + "/Dev/Scripts/Modules/UI/UIBindJs.json";
            if (File.Exists(writePath))
            {
                return JsonConvert.DeserializeObject<UIAssetDict>(File.ReadAllText(writePath));
            }
            return null;
        }
    }
}

