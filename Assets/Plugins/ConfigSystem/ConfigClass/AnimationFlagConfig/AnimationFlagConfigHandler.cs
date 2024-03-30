using System;
using System.Runtime.Serialization;
using LS.Game;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AnimationFlagConfigHandler
{
    [NonSerialized]
    private bool _isDirty = false;
    public bool IsDirty {
        get => _isDirty;
        set => _isDirty = value;
    }

    private int configId = 0;

    [InlineButton(action: "OpenSelector", label: "Set")]
    [ShowInInspector]
    [EnableGUI]
    public int ConfigId
    {
        get
        {
            int weaponId = weaponConfig ? weaponConfig.id : 0;
            int stateId = stateConfig ? stateConfig.id : 0;
            configId = weaponId + stateId;
            return configId;
        }
    }

#if !FLAG_DEBUG
    [HideIf("@true")]
#endif
    public AnimationFlagConfigItem weaponConfig;
#if !FLAG_DEBUG
    [HideIf("@true")]
#endif
    public AnimationFlagConfigItem stateConfig;

    public int WeaponId => weaponConfig ? weaponConfig.id : 0;
    public int StateId => stateConfig ? stateConfig.id : 0;


#if UNITY_EDITOR

    private static string strNone1 = "空手";
    private static string strNone2 = "无";
    [OnInspectorGUI]
    [PropertyOrder(-1)]
    public void ShowStateTips()
    {
        //int weaponId = weaponConfig ? weaponConfig.id : 0;
        string weaponStr = weaponConfig ? weaponConfig.strValue : strNone1;
        //int stateId = stateConfig ? stateConfig.id : 0;
        string stateStr = stateConfig ? stateConfig.strValue : strNone2;
        EditorGUILayout.HelpBox($"[{weaponStr}] {stateStr}", MessageType.None);
    }

    [NonSerialized]
    public UnityEngine.ScriptableObject _host;
    private void OpenSelector()
    {
        AnimationFlagConfigSelector.Show(WeaponId, StateId, OnConfigIdChange);
    }
    private void OnConfigIdChange(AnimationFlagConfigItem item, bool selected)
    {
        if (item == null)
            return;
        AnimationFlagConfigItem value = selected ? item : null;
        if (item.id >= 10000)
        {
            _isDirty = weaponConfig != item;
            weaponConfig = value;
        }
        else
        {
            _isDirty = stateConfig != item;
            stateConfig = value;
        }

        configId = ConfigId;

#if FLAG_DEBUG
        Debug.Log($"[isDirty] {_isDirty} [ConfigId]{configId}");
#endif

        if (_host != null && _isDirty)
        {
#if FLAG_DEBUG
            Debug.Log($"[Save] {_host}");
#endif
            EditorUtility.SetDirty( _host );
            AssetDatabase.Refresh();
        }    
    }
    [OnInspectorInit]
    public void OnInspectorInit(InspectorProperty property)
    {
        _host = property.GetFirstParentOfValue<ScriptableObject>(5);
#if FLAG_DEBUG
        if (_host != null)
            Debug.Log($"[OnInspectorInit] {_host}");
        else
            Debug.Log($"[OnInspectorInit] _host = null");
#endif

    }
#endif
}
