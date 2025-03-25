#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PJR.Systems;

namespace PJR
{
    public class PrefabNameValidateAttributeDrawer : OdinAttributeDrawer<PrefabNameValidateAttribute, string>
    {
        const string Ext_prefab = ".prefab";
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.CallNextDrawer(label);

            string assetName = this.ValueEntry.SmartValue;
            SirenixEditorGUI.BeginBox();
            if (string.IsNullOrEmpty(assetName))
            { 
                GUILayout.Label($"[assetName为空]");
                SirenixEditorGUI.EndBox();
                return;
            }
            string ext = Path.GetExtension(assetName);
            if (string.IsNullOrEmpty(ext))
            {
                GUILayout.Label($"[后缀不能为空]");
                SirenixEditorGUI.EndBox();
                return;
            }
            else if(ext != Ext_prefab)
            {
                GUILayout.Label($"[后缀不是: {Ext_prefab}]");
                SirenixEditorGUI.EndBox();
                return;
            }

            ResourceSystem.Editor_TryGetAssetInfoByName(assetName,out var assetInfo);
            EditorGUILayout.BeginHorizontal();

            if (string.IsNullOrEmpty(assetInfo.Error))
            {
                GUILayout.Label($"[{assetInfo.AssetGUID}]");
                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                    PingByGUID(assetInfo.AssetGUID);
            }
            else
            {
                GUILayout.Label($"[找不到对应Asset]");
            }


            EditorGUILayout.EndHorizontal();

            SirenixEditorGUI.EndBox();
        }

        void PingByGUID(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath))
                return;
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath,typeof(UnityEngine.Object)));

        }
    }
}
#endif
