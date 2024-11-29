using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

[Serializable, HideLabel]
public class FlagHandle256
{
    [NonSerialized]
    private bool _initialized = false;

    [LabelText("FlagID"),OnValueChanged("Editor_OnFlagChanged") , InlineButton("Editor_SelectConfig", label:"选择"), SerializeField]
    private int flagId = -1;
    public int FlagID
    {
        get {
            return flagId;
        }
    }

    public bool Valid
    { 
        get
        {
            if (flagId <= 0)
                return false;
            if (!_initialized)
                return false;
            if (flag.IsEmpty())
                return false;
            return true;
        }
    }

    [NonSerialized]
    private Flag256 flag = Flag256.Empty;
    [NonSerialized, HideReferenceObjectPicker]
    private FlagDefine flagDefine = null;
    [NonSerialized]
    private string flagMenuName = string.Empty;


    public void Init(FlagManager256 flagHandle)
    { 
        if(flagHandle == null)
            throw new NullReferenceException();
        flag = flagHandle.StringToFlag(flagId.ToString());
        _initialized = true;
    }

    public override string ToString()
    {
#if UNITY_EDITOR
        var flagDefine = FlagRuntimeEditorUtil.Editor_GetFlagDefine(flagId);
        if (flagDefine == null)
        {
            return $"[id] :{flagId} (有没有找到对应id的FlagDefine！)";
        }
        return flagDefine.ToString();
#else
        return $"[id] :{flagId}";
#endif
    }
    public static implicit operator int(FlagHandle256 flag)
    { 
        return flag.FlagID;
    }

#if UNITY_EDITOR
    public void Editor_SetFlagID(int flagID)
    {
        this.flagId = flagID;
    }
    private void Editor_OnFlagChanged()
    {
        if (flagDefine != null && flagDefine.ID != FlagID)
        { 
            flagDefine = null;
            flagMenuName = string.Empty;
        }
        if (flagDefine == null && FlagID > 0)
            flagDefine = FlagRuntimeEditorUtil.Editor_GetFlagDefine(FlagID);
        if (flagDefine != null && string.IsNullOrEmpty(flagMenuName))
            flagMenuName = FlagRuntimeEditorUtil.Editor_GetMenuName(FlagID);
    }

    [OnInspectorGUI]
    private void Editor_DrawInfo()
    {
        SirenixEditorGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        Editor_OnFlagChanged();

        if (flagDefine != null)
            GUILayout.Label($"[{flagDefine.ID}][{flagMenuName}]");
        else
            GUILayout.Label($"[找不到id对应配置]");
        EditorGUILayout.EndHorizontal();
        SirenixEditorGUI.EndBox();
    }
    public static void Editor_DrawInfo_Static(FlagHandle256 flagHandle, string header = null)
    {
        if (flagHandle == null)
        {
            if (string.IsNullOrEmpty(header))
                GUILayout.Label($"[找不到id对应配置]");
            else
                GUILayout.Label($"{header} [找不到id对应配置]");
        }
        else
        {
            flagHandle.Editor_OnFlagChanged();

            if (string.IsNullOrEmpty(header))
            {
                if (flagHandle.flagDefine != null)
                    GUILayout.Label($"[{flagHandle.flagDefine.ID}][{flagHandle.flagMenuName}]");
                else
                    GUILayout.Label($"[找不到id对应配置]");
            }
            else
            {
                if (flagHandle.flagDefine != null)
                    GUILayout.Label($"{header} [{flagHandle.flagDefine.ID}][{flagHandle.flagMenuName}]");
                else
                    GUILayout.Label($"{header} [找不到id对应配置]");
            }
        }
    }
    private void Editor_SelectConfig()
    {
        FlagRuntimeEditorUtil.Editor_ShowFlagGenericMenu(flagDefine =>
        {
            flagId = flagDefine.ID;
            this.flagDefine = flagDefine;
        });
    }
#endif
}

[Serializable, HideLabel]
public class FlagHandle512
{
    [NonSerialized]
    private bool _initialized = false;

    [LabelText("FlagID"), OnValueChanged("Editor_OnFlagChanged"), InlineButton("Editor_SelectConfig", label: "选择"), SerializeField]
    private int flagId = -1;
    public int FlagID
    {
        get
        {
            return flagId;
        }
    }

    [NonSerialized]
    private string flagIDStr = string.Empty;
    public string FlagIDStr
    {
        get
        {
            if (flagId <= 0)
                return string.Empty;
            if (string.IsNullOrEmpty(flagIDStr))
                flagIDStr = flagId.ToString();
            return flagIDStr;
        }
    }

    [NonSerialized]
    private Flag512 flag = Flag512.Empty;
    public Flag512 Flag
    {
        get
        {
            if (!_initialized)
            {
                flag = FlagManager512.GetFlag(CategoryID, FlagIDStr);
                _initialized = true;
            }
            if (!Valid)
                return Flag512.Empty;
            return flag;
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
                _flagIDInfo = FlagIDInfo.GetInfo(flagId);
            else if(_flagIDInfo.flagID != flagId)
                _flagIDInfo = FlagIDInfo.GetInfo(flagId);
            return _flagIDInfo;
        }
    }
    public int CategoryID => FlagIDInfo.categoryID;
    public int Category => FlagIDInfo.category;
    public int Kind => FlagIDInfo.kind;
    public int Type => FlagIDInfo.type;

    #endregion

    public bool Valid
    {
        get
        {
            if (flagId <= 0)
                return false;
            if (flag.IsEmpty())
                return false;
            return true;
        }
    }

    public FlagHandle512()
    {
        this.flagId = -1;
    }
    public FlagHandle512(int flagId)
    { 
        this.flagId = flagId;
    }
    public override bool Equals(object obj)
    {
        if (obj != null && obj is FlagHandle512)
        { 
            var rhs = (FlagHandle512)obj;
            return FlagID == rhs.flagId;
        }
        return base.Equals(obj);
    }
    public static bool operator ==(FlagHandle512 lhs, FlagHandle512 rhs)
    { 
        if(ReferenceEquals(lhs, null))
            return ReferenceEquals(rhs, null);
        return lhs.Equals(rhs);
    }
    public static bool operator !=(FlagHandle512 lhs, FlagHandle512 rhs)
    { 
        return !(lhs == rhs);
    }
    public override string ToString()
    {
#if UNITY_EDITOR
        var flagDefine = FlagRuntimeEditorUtil.Editor_GetFlagDefine(flagId);
        if (flagDefine == null)
        {
            return $"[id] :{flagId} (有没有找到对应id的FlagDefine！)";
        }
        return flagDefine.ToString();
#else
        return $"[id] :{flagId}";
#endif
    }
    public static implicit operator int(FlagHandle512 flag)
    {
        return flag.FlagID;
    }

#if UNITY_EDITOR
    [NonSerialized]
    private FlagDefine flagDefine = null;
    [NonSerialized]
    private string flagMenuName = string.Empty;

    public void Editor_SetFlagID(int flagID)
    { 
        this.flagId = flagID;
    }
    private void Editor_OnFlagChanged()
    {
        if (flagDefine != null && flagDefine.ID != FlagID)
        {
            flagDefine = null;
            flagMenuName = string.Empty;
        }
        if (flagDefine == null && FlagID > 0)
            flagDefine = FlagRuntimeEditorUtil.Editor_GetFlagDefine(FlagID);
        if (flagDefine != null && string.IsNullOrEmpty(flagMenuName))
            flagMenuName = FlagRuntimeEditorUtil.Editor_GetMenuName(FlagID);
    }

    [OnInspectorGUI]
    private void Editor_DrawInfo()
    {
        SirenixEditorGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        Editor_OnFlagChanged();

        if (flagDefine != null)
            GUILayout.Label($"[{flagDefine.ID}][{flagMenuName}]");
        else
            GUILayout.Label($"[找不到id对应配置]");
        EditorGUILayout.EndHorizontal();
        SirenixEditorGUI.EndBox();
    }
    public static void Editor_DrawInfo_Static(FlagHandle512 flagHandle, string header = null)
    {
        if (flagHandle == null)
        { 
            if(string.IsNullOrEmpty(header))
                GUILayout.Label($"[找不到id对应配置]");
            else 
                GUILayout.Label($"{header} [找不到id对应配置]");
        }
        else
        {
            flagHandle.Editor_OnFlagChanged();

            if (string.IsNullOrEmpty(header))
            {
                if (flagHandle.flagDefine != null)
                    GUILayout.Label($"[{flagHandle.flagDefine.ID}][{flagHandle.flagMenuName}]");
                else
                    GUILayout.Label($"[找不到id对应配置]");
            }
            else
            {
                if (flagHandle.flagDefine != null)
                    GUILayout.Label($"{header} [{flagHandle.flagDefine.ID}][{flagHandle.flagMenuName}]");
                else
                    GUILayout.Label($"{header} [找不到id对应配置]");
            }
        }
    }
    public string Editor_GetMenuName(bool onlyLastSection = false)
    {
        Editor_OnFlagChanged();

        if (flagDefine != null)
        {
            if (onlyLastSection)
            {
                int index = flagDefine.Name.LastIndexOf('/');
                if (index >= 0)
                    return flagDefine.Name.Substring(index + 1);
                return flagMenuName;
            }
            return flagMenuName;
        }
        else
            return $"[{FlagID}][找不到id对应配置]";
    }
    public string Editor_GetConfig_Key(bool onlyLastSection = false)
    {
        Editor_OnFlagChanged();

        if (flagDefine != null)
            if (!string.IsNullOrEmpty(flagDefine.Key))
            {
                if (onlyLastSection)
                { 
                    int index = flagDefine.Key.LastIndexOf('.');    
                    if(index >= 0)
                        return flagDefine.Key.Substring(index + 1);
                    return flagDefine.Key;
                }
                return flagDefine.Key;
            }
            else
                return $"[{FlagID}][没有配Key]";
        else
            return $"[{FlagID}][找不到id对应配置]";
    }
    private void Editor_ResetCacheField()
    {
        flagIDStr = string.Empty;
        flag = Flag512.Empty;
        _initialized = false;
    }
    private void Editor_SelectConfig()
    {
        FlagRuntimeEditorUtil.Editor_ShowFlagGenericMenu(flagDefine =>
        {
            flagId = flagDefine.ID;
            this.flagDefine = flagDefine;
            flagMenuName = FlagRuntimeEditorUtil.Editor_GetMenuName(flagId);
            Editor_ResetCacheField();
        }, -1);
    }
    private void Editor_SelectFilteredConfig(bool categoryOnly = false, params int[] categorys)
    {
        FlagRuntimeEditorUtil.Editor_ShowFilteredFlagGenericMenu(flagDefine =>
        {
            flagId = flagDefine.ID;
            flagMenuName = string.Empty;
            this.flagDefine = flagDefine;
        }, categoryOnly, categorys);
    }
    public bool Editor_OnGUI(string flagIDLabel = null, bool categoryOnly = false, params int[] menuCategorys)
    {
        EditorGUI.BeginChangeCheck();
        if (string.IsNullOrEmpty(flagIDLabel))
            flagIDLabel = "FlagID";
        EditorGUILayout.BeginHorizontal();
        flagId = EditorGUILayout.IntField(new GUIContent(flagIDLabel), flagId);
        if (GUILayout.Button("选择", GUILayout.Width(60f)))
        {
            Editor_SelectFilteredConfig(categoryOnly, menuCategorys);
        }
        EditorGUILayout.EndHorizontal();
        Editor_DrawInfo();
        return EditorGUI.EndChangeCheck();
    }
#endif
}

