using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class LevelSelectWindowUI : UINode
    {
        public Button fullCloseBtn;
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
        }
    }
}

