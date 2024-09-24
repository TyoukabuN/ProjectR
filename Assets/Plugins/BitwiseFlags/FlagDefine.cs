using System;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

[Serializable]
public partial class FlagDefine : IFlagDefine
{
    [SerializeField]
    public int id = -1;
    [LabelText("名字(菜单选项)")]
    public string nameStr = null;
    [LabelText("Key(脚本生成用)")]
    public string Key = null;
    public override string ToString()
    {
        return $"[id]; {id}\n[nameStr]; {nameStr}\n[Key]; {Key}";
    }

    [NonSerialized]
    private string idStr = string.Empty;
    public string IDStr
    {
        get {
            if (id <= 0)
                return string.Empty ;
            if (string.IsNullOrEmpty(idStr))
                idStr = id.ToString();
            return idStr ;
        }
    }

    #region FlagIDInfo

    [NonSerialized]
    private FlagIDInfo _flagIDInfo;
    public FlagIDInfo FlagIDInfo
    {
        get
        {
            if (_flagIDInfo == null)
                _flagIDInfo = FlagIDInfo.GetInfo(ID);
            return _flagIDInfo;
        }
    }
    public int CategoryID => FlagIDInfo.categoryID;
    public int Category => FlagIDInfo.category;
    public int Kind => FlagIDInfo.kind;
    public int Type => FlagIDInfo.type;

    #endregion


    #region IFlagDefine
    public int ID => id;
    public string Name => nameStr;
    public bool IsValid()
    { 
       if(string.IsNullOrEmpty(Key))
            return false;
       return true;
    }
    public string GetKey()
    {
        if (!IsValid())
            return string.Empty;
        return Key;
    }

    private string[] keys = null;
    public string[] GetKeys()
    {
        if (keys == null) { 
            keys = new string[1] { Key };
        }
        return keys;
    }
    #endregion

#if UNITY_EDITOR
    public bool OnGUI(string flagIDLabel = null)
    {
        EditorGUI.BeginChangeCheck();
        if (string.IsNullOrEmpty(flagIDLabel))
            flagIDLabel = "FlagID";
        EditorGUILayout.BeginHorizontal();
        id = EditorGUILayout.IntField(new GUIContent(flagIDLabel), id);
        if (GUILayout.Button("选择", GUILayout.Width(60f)))
        {
            //Editor_SelectFilteredConfig(menuCategory);
        }
        EditorGUILayout.EndHorizontal();

        SirenixEditorGUI.BeginBox();
        GUILayout.Label($"[{ID}][{Name}]");
        SirenixEditorGUI.EndBox();

        return EditorGUI.EndChangeCheck();
    }
#endif
}

public interface IFlagDefine
{
    public int ID { get; }
    public string Name { get; }
    public bool IsValid();
    public string GetKey();
    public string[] GetKeys();
}
