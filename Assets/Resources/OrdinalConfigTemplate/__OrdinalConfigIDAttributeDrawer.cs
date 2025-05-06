using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR.Config
{
    public class __OrdinalConfigIDAttributeDrawer : OdinAttributeDrawer<__OrdinalConfigIDAttribute, int>
    {
        private bool _enablePropTreeGroup = false;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
            int id = ValueEntry.SmartValue;
            SirenixEditorGUI.BeginBox();
            var itemAsset = __OrdinalConfig.Instance.GetConfig(id);
            EditorGUILayout.BeginHorizontal();
            if (itemAsset != null)
            {
                if (GUILayout.Button(itemAsset.Editor_LabelName))
                {
                    if (__OrdinalConfig.Instance.Editor_GetItemAsset(id) is { } a)
                    {
                        if (Event.current.control) OdinEditorWindow.InspectObject(a);
                        else EditorGUIUtility.PingObject(a);
                    }
                }
            }
            else
                GUILayout.Label($"[找不到id对应配置]");


            if (GUILayout.Button("更多", GUILayout.Width(48)))
            {
                ShowMoreMenu(itemAsset);
            }

            if (GUILayout.Button("新建", GUILayout.Width(48)))
            {
                __OrdinalConfig.Instance.Editor_OpenItemCreateWindow((config) =>
                {
                    ValueEntry.SmartValue = config.ID;
                    ValueEntry.ApplyChanges();
                });
            }

            if (GUILayout.Button("复制", GUILayout.Width(48)))
            {
                if (itemAsset != null)
                {
                    var errCode = __OrdinalConfig.Instance.Editor_CopyItemAsset(itemAsset, out var copy);
                    if (string.IsNullOrEmpty(errCode))
                    {
                        ValueEntry.SmartValue = copy.ID;
                        ValueEntry.ApplyChanges();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Tips", errCode, "OK");
                    }
                }
            }

            if (GUILayout.Button("选择", GUILayout.Width(48)))
            {
                __OrdinalConfig.Selector.Show((config) =>
                {
                    ValueEntry.SmartValue = config.ID;
                    ValueEntry.ApplyChanges();
                });
            }

            EditorGUILayout.EndHorizontal();
            if (itemAsset != null)
            {
                var key = UniqueDrawerKey.Create(this.Property, this);
                _enablePropTreeGroup = SirenixEditorGUI.Foldout(_enablePropTreeGroup, "详细");
                if (_enablePropTreeGroup)
                {
                    var propertyTree = __OrdinalConfig.Instance.Editor_GetPropertyTreeByID(itemAsset.ID);
                    if (propertyTree != null)
                    {
                        propertyTree.BeginDraw(true);
                        propertyTree.DrawProperties();
                        if (propertyTree.ApplyChanges())
                        {
                            propertyTree.DelayAction(() =>
                            {
                                __OrdinalConfig.Instance.Editor_MaskItemAssetDirty(itemAsset.ID, false);
                            });
                        }

                        propertyTree.EndDraw();
                    }
                }
            }

            SirenixEditorGUI.EndBox();
        }

        public void ShowMoreMenu(__OrdinalConfigItemAsset item)
        {
            __OrdinalConfig.Instance.Editor_ShowMoreMenu(menu =>
            {
                if (item != null)
                    menu.AddItem(new GUIContent("删除"), false,
                        () => { __OrdinalConfig.Instance.Editor_RemoveItem(item.ID); });
            });
        }
    }
}