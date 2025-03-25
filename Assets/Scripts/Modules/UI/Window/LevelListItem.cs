using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class LevelListItem : UIListItem
    {
        public GameObject lockObj;
        public Button startButton;
        public Text time;
        public override void OnStart()
        {
        }
        public override void OnData(UIListItemData uData)
        {
            base.OnData(uData);
            Refresh();
        }
        public void Refresh()
        {
            lockObj.SetActive((bool)data.GetData("islock"));
            if (data.GetData("bestTime")!=null)
            {
                time.text = ((float)data.GetData("bestTime")).ToString();
            }
        }
        
    }
}

