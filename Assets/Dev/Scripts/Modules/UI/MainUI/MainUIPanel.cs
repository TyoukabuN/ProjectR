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
                md.message = $"the message from {this.name}";
                UISystem.instance.OpenPanel("LevelSelectWindow", md);
            });
            optionBtn.onClick.AddListener(() =>
            {
                UISystem.instance.OpenPanel("OptionWindowPanel");
            });
        }
    }
    public class MainUIData:UINodeData
    {
        public string message;
        public List<int> levels = new List<int> { 1,2,3,4};
    }
}

