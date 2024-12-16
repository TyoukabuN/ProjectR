using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PJR.Systems.ResourceSystem;
using PJR.ClassExtension;

namespace PJR.Systems
{
    public enum UILayer
    {
        Main = 0, //全屏界面
        Game, //游戏中界面
        Top, //弹窗
        MessageTop, //最顶级
    }
    public class UISystem : MonoSingletonSystem<UISystem>
    {
        public override Vector3 Position => new Vector3(5000, 5000, 5000);

        public GameObject UIRoot;
        //key : GameObjectInstanceID
        public Dictionary<UILayer, Dictionary<int, UINode>> nodeDict = new Dictionary<UILayer, Dictionary<int, UINode>>();
        //根节点字典
        public Dictionary<UILayer, Transform> rootDict = new Dictionary<UILayer,Transform>();
        //按名字储存的实例
        public Dictionary<string,UINode> nameUINodeDict = new Dictionary<string, UINode>();
        public Transform CanvasRoot;

        public GameObject EventSystemObj;

        public Camera UICamera;
        public UIAssetDict UIAssetdict;
        public override IEnumerator Initialize()
        {
            UIAssetdict = UIAssetDataFunc.GetBindUIAssetDict();
            CreateRoot();
            CreateEventSystem();
            CreateUICamera();
            TestEntrance();
            LogSystem.Log("========UIRoot准备完成");

            yield return null;
        }
        private void CreateRoot()
        {
            UIRoot = this.gameObject;
            Type[] types = { typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler) };
            GameObject canvasObj = new GameObject("Canvas", types);
            canvasObj.layer = 5;
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            canvasObj.transform.SetParent(UIRoot.transform);
            CanvasRoot = canvasObj.transform;
            CanvasRoot.position = Vector3.zero;
            GenObject(UILayer.Main, "MainUIRoot");
            GenObject(UILayer.Game, "GameUIRoot");
            GenObject(UILayer.Top, "TopUIRoot");
            GenObject(UILayer.MessageTop, "MessageTopUIRoot");
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
        private void CreateUICamera()
        {
            if (UICamera == null)
            {
                GameObject obj = new GameObject("UICamera", typeof(Camera));
                UICamera = obj.GetComponent<Camera>();
                if (CanvasRoot != null)
                {
                    CanvasRoot.GetComponent<Canvas>().worldCamera = UICamera;
                }
                UICamera.clearFlags = CameraClearFlags.Depth;
                UICamera.cullingMask = 1<<5;
                UICamera.orthographic = true;
                UICamera.orthographicSize = 100;
                UICamera.nearClipPlane = -1000;
                UICamera.farClipPlane = 200;
                UICamera.transform.SetParent(UIRoot.transform,false);
                UICamera.transform.localScale = Vector3.one;
            }
        }
        public void TestEntrance()
        {
            OpenPanel("MainPanel");
        }
        Dictionary<UILayer,float> layerFar = new Dictionary<UILayer, float> { 
            [UILayer.Main] = 0f,
            [UILayer.Game] = -200f,
            [UILayer.Top] = -600f,
            [UILayer.MessageTop] = -900f,
        };
        private void GenObject(UILayer uilayer,string name)
        {
            GameObject obj = new GameObject(name,typeof(RectTransform));
            obj.layer = 5;
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.localPosition = new Vector3(0, 0, layerFar[uilayer]);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            obj.transform.SetParent(CanvasRoot);
            rootDict[uilayer] = obj.transform;
        }
        public void ShowNormal(UINode node,object data = null)
        {
            Array arr = Enum.GetValues(typeof(UILayer));
            
            for (int i = 0; i < arr.Length; i++)
            {
                if (node.layer == (UILayer)arr.GetValue(i))
                {
                    foreach (var item in nodeDict[node.layer])
                    {
                        if (item.Value == node)
                        {
                            item.Value.gameObject.SetActive(true);
                            item.Value.gameObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            item.Value.gameObject.SetActive(false);
                        }
                    }
                    break;
                }
            }
            node.OnOpen();
            if (data !=null)
            {
                SetData(node.UIName, data);
            }
        }
        
        public void Close(UINode node,bool isRelease)
        {
            if (isRelease)
            {
                node.OnClose();
                node.OnDestory();
                nodeDict[node.layer].Remove(node.instanID);
                nameUINodeDict.Remove(node.UIName);
                Destroy(node.gameObject);
            }
            else
            {
                node.OnClose();
                node.gameObject.SetActive(false);
            }
        }
        public void OpenPanel(string name,object data =null)
        {
            if (UIAssetdict.assets.ContainsKey(name))
            {
                if (!nameUINodeDict.ContainsKey(name))
                {
                    LoadUI(UIAssetdict.assets[name].prefab,data,true);
                }
                else
                {
                    ShowNormal(nameUINodeDict[name],data);
                }
            }
            else
            {
                LogSystem.LogError($"不存在名称{name}的UIPanel");
            }
        }
        private void LoadUI(string path,object data,bool isDoneShow = false)
        {
            StartCoroutine(LoadAsset(path, data,isDoneShow));
        }
        IEnumerator LoadAsset(string name, object data, bool isDoneShow)
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
                OnLoadDone(loader,data, isDoneShow);
            }
        }
        public void OnLoadDone(ResourceLoader loader,object data,bool isDoneShow)
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
                nameUINodeDict[uinode.UIName] = uinode;
                //重设rect
                ui.transform.SetParent(rootDict[uinode.layer]);
                RectTransform rect = ui.TryGetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

                uinode.OnInit();
                ui.SetActive(false);
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
                    UINode node = nameUINodeDict[uinode.UIName];
                    node.data = data;
                    if (node !=null)
                    {
                        ShowNormal(node,data);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recive">接受data的panel名字</param>
        /// <param name="data"></param>
        public void SetData(string recive,object data)
        {
            if (nameUINodeDict.ContainsKey(recive))
            {
                UINode node = nameUINodeDict[recive];
                node.data= data;
                node.OnData(data);
            }
            else
            {
                LogSystem.LogError($"{recive}不存在");
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

