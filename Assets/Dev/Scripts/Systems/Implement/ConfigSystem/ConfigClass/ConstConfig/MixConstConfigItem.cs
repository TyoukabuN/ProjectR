using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public class MixConstConfigItem
    {
        public enum ValueType
        { 
            [LabelText("空")]None,
            [LabelText("整数")]Int,
            [LabelText("浮点")]Float,
            [LabelText("字符串")]String,
            [LabelText("曲线")]Curve,
        }

        [HideLabel]
        [TableColumnWidth(56, Resizable = false)]
        public ValueType type = ValueType.None;

        [TableColumnWidth(0)]
        public string key = "key";

        [TableColumnWidth(60)]
        [LabelText("描述")]
        public string desc = "desc";

        [ShowIf("@type == ValueType.Int")]public int IntValue;
        [ShowIf("@type == ValueType.Float")]public float FloatValue;
        [ShowIf("@type == ValueType.String")]public string StringValue;
        [ShowIf("@type == ValueType.Curve")] public AnimationCurve CurveValue;
    }
}
