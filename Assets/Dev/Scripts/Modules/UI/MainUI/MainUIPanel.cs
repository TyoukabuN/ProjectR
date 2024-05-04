using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class MainUIPanel :UINode
    {
        public Button startBtn;
        public Button optionBtn;
        public Button exitBtn;
        public override void OnStart()
        {
            base.OnStart();
            startBtn.onClick.AddListener(() => { 
                MainUIData md = new MainUIData();
                md.name = "gawgwasgwa";
                UISystem.instance.OpenPanel("NorUI",md);
            });
            optionBtn.onClick.AddListener(() =>
            {
                UISystem.instance.OpenPanel("OptionWindowPanel");
            });
        }
    }
    public class MainUIData:UINodeData
    {
        public string name;
    }
}

