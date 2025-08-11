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
            closeBtn.onClick.AddListener(() => { Close(true); });
        }
        public override void OnOpen()
        {
            Debug.Log("NormalUIPanel����");
        }
        public override void OnData(object data)
        {
            base.OnData(data);
            if (data is string)
            {
                Debug.Log(data);
            }
            if (data is UINodeData)
            {
                MainUIData mud = (MainUIData)data;
                Debug.Log(mud.message);
            }
        }
    }
}

