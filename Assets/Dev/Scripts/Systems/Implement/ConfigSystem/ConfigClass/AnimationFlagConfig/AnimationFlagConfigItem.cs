using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[InlineProperty]
[System.Serializable]
public class AnimationFlagConfigItem : ScriptableObject
{
    [ValidateInput("id_valid", "动画id需要大于0, 主手武器id需要是10000的倍数")]
    [LabelText("id")]
    public int id;

    [ValidateInput("strValue_valid", "字符串不能为空")]
    public string strValue;

    #region Odin Valid
    private bool id_valid(int value)
    {
        return value >= 0;
    }
    private bool strValue_valid(string value)
    {
        return !string.IsNullOrEmpty(value);
    }
    #endregion

    public static readonly AnimationFlagConfigItem _Empty = null;

    public static readonly AnimationFlagConfigItem Empty = _Empty??= new AnimationFlagConfigItem(0, string.Empty);

    public AnimationFlagConfigItem(int id, string strValue)
    {
        this.id = id;
        this.strValue = strValue;
    }

    public bool IsEmpty()
    {
        if (this == Empty)
           return true;
        if(id <= 0)
           return true;
        if (string.IsNullOrEmpty(strValue))
            return true;
        return false;
    }

    public static implicit operator KeyValuePair<int, AnimationFlagConfigItem>(AnimationFlagConfigItem item)
    {
        return new KeyValuePair<int, AnimationFlagConfigItem>(item.id, item);
    }

    public string ToString()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"[ToString] {this.GetType().Name} = ");
        sb.AppendLine($"{{");

        foreach (FieldInfo field in this.GetType().GetFields())
        {
            // 获取字段的名称和值
            string fieldName = field.Name;
            object fieldValue = field.GetValue(this);
            sb.AppendLine(string.Format("{0} = {1}", fieldName, fieldValue));
        }
        sb.AppendLine($"}}");
        return sb.ToString();
    }
}
