using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR.Systems;
using UnityEngine.UI;

namespace PJR
{
    public class UINode : MonoBehaviour
    {
        public UILayer layer;
        [LabelText("panel名字")]
        [Tooltip("必须唯一")]
        public string UIName = "";
        public string prefab;
        [ReadOnly]
        public int instanID;
        [ReadOnly]
        public UINode parent;
        public object data;
        private void Start()
        {
            OnStart();
        }
        public virtual void OnInit()
        {
            instanID = this.gameObject.GetInstanceID();
        }
        public virtual void OnStart()
        {
            transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
        public virtual void OnData(object data)
        {
            if (data is UINode)
            {
                parent = (UINode)data;
            }
        }
        public virtual void OnOpen()
        {

        }
        public virtual void OnClose()
        {

        }
        public virtual void OnDestory()
        {

        }
        public virtual void Close(bool isRelease = false)
        {
            UISystem.instance.Close(this, isRelease);
        }
        
    }
    /// <summary>
    /// panel数据
    /// </summary>
    public class UINodeData
    {

    }
    /// <summary>
    /// 自定义字典数据
    /// </summary>
    /// <typeparam name="Value"></typeparam>
    public class DataDict<Value>
    {
        public string key;
        public Value value;
        public DataDict(string key, Value v)
        {
            this.key = key;
            this.value = v;
        }

    }

    [Serializable]
    public class UIAssetDict
    {
        [SerializeField]
        public Dictionary<string,UIAsset> assets = new Dictionary<string, UIAsset>();
    }
    [Serializable]
    public class UIAsset
    {
        //Ψһ
        [SerializeField]
        public string name;
        [SerializeField]
        public string prefab;
        //[SerializeField]
        //public UINode node;

        public UIAsset(string name ,string prefab)
        {
            this.name = name;
            this.prefab = prefab;
            //this.node = node;
        }
    }
}

