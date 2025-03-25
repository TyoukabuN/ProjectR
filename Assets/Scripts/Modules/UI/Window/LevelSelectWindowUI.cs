using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class LevelSelectWindowUI : UINode
    {
        public Button fullCloseBtn;
        public UIListView listView;
        public override void OnStart()
        {
            fullCloseBtn.onClick.AddListener(() => Close());
        }
        public override void OnData(object data)
        {
            if (data is MainUIData)
            {
                MainUIData info = (MainUIData)data;
                Debug.Log(info.message);
            }
            TestDeal();
        }
        /// <summary>
        /// 测试输入数据
        /// 看到时候配置怎样再写读配置的方法
        /// </summary>
        public void TestDeal()
        {
            UIListItemData uIListItemData = new UIListItemData();
            uIListItemData.SetData("level", 1);
            uIListItemData.SetData("islock", false);
            uIListItemData.SetData("bestTime", 99f);
            UIListItemData uIListItemData2 = new UIListItemData();
            uIListItemData2.SetData("level", 2);
            uIListItemData2.SetData("islock", true);
            UIListItemData uIListItemData3 = new UIListItemData();
            uIListItemData3.SetData("level", 3);
            uIListItemData3.SetData("islock", true);
            List<UIListItemData> list = new List<UIListItemData>();
            list.Add(uIListItemData);
            list.Add(uIListItemData2);
            list.Add(uIListItemData3);
            UIListViewData uvd = new UIListViewData(list);
            listView.SetData(uvd);
        }
    }
}

