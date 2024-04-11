using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[InlineEditor]
public class TestDataItem : SerializedScriptableObject
{
    [LabelText("ID")]
    public int id;
    [LabelText("字符串")]
    public string strValue;
}