using Sirenix.OdinInspector;
using System;

namespace PJR
{
    [Serializable]
    public class ConstConfigItem<Type>
    {
        [HorizontalGroup("Main")]
        [HorizontalGroup("Main/key"), HideLabel]
        public string key = "英文key";

        [HorizontalGroup("Main/value"),HideLabel]
        public Type value;

        [HorizontalGroup("Main/desc"), HideLabel]
        public string desc = "描述";
    }
}
