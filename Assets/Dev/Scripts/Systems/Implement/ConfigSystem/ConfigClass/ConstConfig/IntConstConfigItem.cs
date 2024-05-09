using Sirenix.OdinInspector;
using System;

namespace PJR
{
    [Serializable]
    public class IntConstConfigItem
    {
        [HorizontalGroup("Main")]
        [HorizontalGroup("Main/key"), HideLabel]
        public string key = "key";

        [HorizontalGroup("Main/IntValue"),HideLabel]
        public int IntValue;

        [HorizontalGroup("Main/desc"), HideLabel]
        public string desc = "描述";
    }
}
