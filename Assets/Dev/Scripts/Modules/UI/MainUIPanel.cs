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
            startBtn.onClick.AddListener(() => { Debug.Log("-----µã»÷ÁË"); });
        }
    }
}

