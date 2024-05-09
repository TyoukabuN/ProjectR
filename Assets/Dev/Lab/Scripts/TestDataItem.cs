using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
[InlineEditor]
public class TestDataItem : ScriptableObject
{
    [LabelText("ID")]
    public int id;
    [LabelText("字符串")]
    public string strValue;
}