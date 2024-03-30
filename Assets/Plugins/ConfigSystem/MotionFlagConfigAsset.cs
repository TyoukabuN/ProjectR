using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[CreateAssetMenu(fileName = "MotionFlagConfigAsset", menuName = "ConfigsAsset/MotionFlagConfigAsset", order = 7)]
public class MotionFlagConfigAsset : ConfigAsset<int,MotionFlagConfigItem>
{
    public override void AddElement()
    {
        items.Add(new MotionFlagConfigItem());
    }
    public override void RemoveAt(int index)
    {
        items.RemoveAt(index);
    }
}

public struct MotionFlagConfigItem
{
    public int id;
    public int category;
    public string strValue;
    public string categoryStr;
    public string desc;
    public string icon;

    public static readonly MotionFlagConfigItem Empty = new() {
           id = -1, 
           category = -1,
           strValue = String.Empty,
           categoryStr = String.Empty,
           desc = String.Empty,
           icon = String.Empty,
        };

    public bool IsEmpty()
    {
        return this == Empty;
    }

    public static bool operator ==(MotionFlagConfigItem lhs,MotionFlagConfigItem rhs)
    {
        if(lhs.id != rhs.id) return false;  
        if(lhs.category != rhs.category) return false; 
        if(lhs.strValue != rhs.strValue) return false; 
        if(lhs.categoryStr != rhs.categoryStr) return false; 
        if(lhs.desc != rhs.desc) return false; 
        if(lhs.icon != rhs.icon) return false; 
            
        return true;
    }
    
    public static bool operator !=(MotionFlagConfigItem lhs,MotionFlagConfigItem rhs)
    {
        if(lhs.id == rhs.id) return false;  
        if(lhs.category == rhs.category) return false; 
        if(lhs.strValue == rhs.strValue) return false; 
        if(lhs.categoryStr == rhs.categoryStr) return false; 
        if(lhs.desc == rhs.desc) return false; 
        if(lhs.icon == rhs.icon) return false; 
            
        return true;
    }

    public static implicit operator KeyValuePair<int,MotionFlagConfigItem>(MotionFlagConfigItem item)
    {
        return new KeyValuePair<int, MotionFlagConfigItem>(item.id, item);
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
            sb.AppendLine(string.Format("{0} = {1}",fieldName,fieldValue));
        }
        sb.AppendLine($"}}");
        return sb.ToString();
    }
}
