#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LS.Game;

public class EffectConfigIDAttributeDrawer : OdinAttributeDrawer<EffectConfigIDAttribute, int>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        this.CallNextDrawer(label);

        int id = this.ValueEntry.SmartValue;
        SirenixEditorGUI.BeginBox();
        var config = EffectConfig.GetEffectConfig(id);
        EditorGUILayout.BeginHorizontal();
        if (config != null)
            GUILayout.Label($"[{config.ID}][{config.AssetName}]");
        else
            GUILayout.Label($"[找不到id对应配置]");

        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            EffectConfigSelector.Show((config) =>
            {
                this.ValueEntry.SmartValue = config.ID;
                this.ValueEntry.ApplyChanges();
            });
        }
        EditorGUILayout.EndHorizontal();

        SirenixEditorGUI.EndBox();
    }
}

#endif
