using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[InlineEditor]
public class FlagDefineSet : SerializedScriptableObject, IFlagDefine
{
    public int id = -1;
    [LabelText("名字(菜单选项)")]
    public string nameStr = null;
    [LabelText("Key(脚本生成用)")]
    public string Key = null;
    [ListDrawerSettings(CustomAddFunction = "Add", CustomRemoveIndexFunction = "RemoveAt",OnEndListElementGUI = "OnEndListElementGUI")]
    public List<FlagDefine> FlagDefines = new List<FlagDefine>();

#if UNITY_EDITOR
    public const string CXT_ID_Invalid = "ID失效";
    public const string CXT_Name_Empty = "没有填菜单选项";
    public const string CXT_Key_Empty = "Key为空";

    private void OnEndListElementGUI(int index)
    {
        var flagDefine = FlagDefines[index];
        if (flagDefine == null)
            return;
        int id = flagDefine.id;
        string name = flagDefine.nameStr;
        string key = flagDefine.Key;
        string error = string.Empty;
        string warming = string.Empty;

        if (id < 0)
            error = CXT_ID_Invalid;
        else if (string.IsNullOrEmpty(name))
            warming = CXT_Name_Empty;
        else if (string.IsNullOrEmpty(key))
            warming = CXT_Key_Empty;
        if(!string.IsNullOrEmpty(error))
            EditorGUILayout.HelpBox(error, MessageType.Error);
        else if(!string.IsNullOrEmpty(warming))
            EditorGUILayout.HelpBox(warming, MessageType.Warning);
    }
    private void Add()
    {
        int id = Editor_GenNewID();

        var flagDefine = new FlagDefine();
        flagDefine.id = id;
        flagDefine.nameStr = string.Empty;
        flagDefine.Key = string.Empty;
        FlagDefines.Add(flagDefine);

        SaveAsset();
    }

    private void RemoveAt(int index)
    {
        if (EditorUtility.DisplayDialog("Tips", $"删除配置后，{id}相关的Editor的显示功能将失效\n但输入/状态等{id}作为Flag使用的功能依然有效", "继续删除", "取消"))
        { 
            FlagDefines.RemoveAt(index);

            SaveAsset();
        }
    }

    private void SaveAsset() {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public int Editor_GenNewID()
    {
        var setIDInfo = FlagIDInfo.GetInfo(this.id);
        if(!setIDInfo.IsValid())
            return -1;

        int maxID = FlagDefines.Max(define => define.ID);
        var maxIDInfo = FlagIDInfo.GetInfo(maxID);
        if (!maxIDInfo.IsValid())
            return -1;

        return maxID + 1;
    }
#endif


    #region IFlagDefine
    public int ID => id;
    public string Name => nameStr;
    public bool IsValid()
    { 
        if(FlagDefines == null )
            return true;
        if(FlagDefines.Count <= 0)
            return true;
        return false;
    }
    public string GetKey()
    {
        if(IsValid())
            return FlagDefines[0].GetKey();
        return null;
    }

    [NonSerialized]
    private string[] keys = null;
    public string[] GetKeys()
    {
        if(keys != null)
            return keys;
        //
        if (IsValid())
        { 
            if(keys == null) 
                keys = new string[0]; 
            return keys;
        }
        //
        if(keys == null)
            keys = new string[FlagDefines.Count];
        for(int i = 0; i < FlagDefines.Count; i++)
        {
            if (!FlagDefines[i].IsValid())
                continue;
            keys[i] = FlagDefines[i].GetKey();
        }
        return keys;
    }
    #endregion
}
