using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
