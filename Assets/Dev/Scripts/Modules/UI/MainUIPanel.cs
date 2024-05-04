using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class MainUIPanel :UINode
    {
        public Button startBtn;
        public override void OnStart()
        {
            base.OnStart();
            startBtn.onClick.AddListener(() => { 
                MainUIData md = new MainUIData();
                md.name = "gawgwasgwa";
                UISystem.instance.OpenPanel("NorUI",md);
                //UISystem.instance.SetData("NorUI", "123123");
            });
        }
    }
    public class MainUIData:UINodeData
    {
        public string name;
    }
}

