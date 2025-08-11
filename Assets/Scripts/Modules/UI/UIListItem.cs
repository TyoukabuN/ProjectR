using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class UIListItem : MonoBehaviour
    {
        public int InstanceID;
        public UIListItemData data;
        public virtual void OnData(UIListItemData uData)
        {
            BindData(uData);
        }
        public void Start()
        {
            InstanceID = gameObject.GetInstanceID();
            OnStart();
        }
        public virtual void OnStart() { }
        public void BindData(UIListItemData uData)
        {
            data = uData;
        }
    }
    public class UIListItemData
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();
        public Dictionary<string, object> Data
        { get { return data; } }
        public void SetData(string key, object value)
        {
            if (data.ContainsKey(key))
            {
                Debug.LogError($"[{this.GetType().Name}]{key}�Ѿ�����");
                return;
            }
            data[key] = value;
        }
        public object GetData(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            return null;
        }
    }
}

