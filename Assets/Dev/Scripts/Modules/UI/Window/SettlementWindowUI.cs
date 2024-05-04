using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class SettlementWindowUI : UINode
    {
        public Button closeBtn;
        public Button nextBtn;
        public Button backBtn;
        public Text clearTimeTxt;
        public Text killBossTimeTxt;
        public Text jugeTxt;
        public Text bestTimeTxt;
        public Image newRecordImg;
        public override void OnStart()
        {
            closeBtn.onClick.AddListener(()=>Close());
            backBtn.onClick.AddListener(() => UISystem.instance.OpenPanel("MainUI"));
        }
    }
}

