using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace PJR
{
    public class UIListView : MonoBehaviour
    {
        [LabelText("itemµÄÔ¤ÖÆ")]
        public GameObject itemPrefab;
        public Transform Content;
        public object data;
        [ReadOnly]
        public int instanceID;
        [ReadOnly]
        public List<UIListItem> uIListItems = new List<UIListItem>();
        public void SetData(object data)
        {
            this.data = data;
            if (data is UIListViewData)
            {
                UIListViewData dvd = (UIListViewData)data;
                int lessCount = dvd.dataList.Count - uIListItems.Count;
                if (lessCount > 0)
                {
                    for (int i = 0; i < lessCount; i++)
                    {
                        GenItem();
                    }
                }
                if (lessCount < 0)
                {
                    for (int i = 0; i < -lessCount; i++)
                    {
                        Destroy(uIListItems[uIListItems.Count - 1 - i].gameObject);
                        uIListItems.RemoveAt(uIListItems.Count - 1 - i);
                    }
                }
                for (int i = 0; i < uIListItems.Count; i++)
                {
                    SetItemData(i, dvd.dataList[i]);
                }
            }
        }
        private void Start()
        {
            instanceID = gameObject.GetInstanceID();
        }
        private void GenItem()
        {
            GameObject obj = GameObject.Instantiate(itemPrefab);
            obj.transform.parent = Content;
            obj.transform.localPosition = Vector3.zero; 
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            UIListItem listItem = obj.TryGetComponent<UIListItem>();
            uIListItems.Add(listItem);
        }
        public void SetItemData(int index,UIListItemData itemData)
        {
            uIListItems[index].OnData(itemData);
        }
        public int GetItemIndex(UIListItem item)
        {
            return uIListItems.IndexOf(item);
        }
    }
    public class UIListViewData
    {
        public List<UIListItemData> dataList = new List<UIListItemData>();
        public UIListViewData(List<UIListItemData> data)
        {
            dataList = data;
        }
        public void RemoveByIndex(int index)
        {
            dataList.RemoveAt(index);
        }
        public void Remove(UIListItemData item)
        {
            if (dataList.Contains(item))
                dataList.Remove(item);
        }
        public void Add(UIListItemData item, int index = -1)
        {
            if (index > -1)
            {
                dataList.Insert(index, item);
            }
            else
            {
                dataList.Add(item);
            }
        }
        public void Sort(List<UIListItemData> datalist) { }
    }
    
    
}
