using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class NormalUIPanel : UINode
    {
        public Button closeBtn;
        public override void OnStart()
        {
            base.OnStart();
            closeBtn.onClick.AddListener(() => { Close(); });
        }
    }
}

