#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using EDisplayOption = PJR.Config.OrdinalConfigIDAttribute.EDisplayOption;

namespace PJR.Config
{
    public class AvatarConfigIDAttributeDrawer : OdinAttributeDrawer<AvatarConfigIDAttribute, int>
    {
        private bool _enablePropTreeGroup = false;
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
            int id = ValueEntry.SmartValue;
            SirenixEditorGUI.BeginBox();
            var itemAsset = AvatarConfig.Instance.GetConfig(id);
            EditorGUILayout.BeginHorizontal();
            if (itemAsset != null)
            {
                if (GUILayout.Button(itemAsset.Editor_LabelName))
                {
                    if (AvatarConfig.Instance.Editor_GetItemAsset(id) is { } a)
                    {
                        if (Event.current.control) OdinEditorWindow.InspectObject(a);
                        else EditorGUIUtility.PingObject(a);
                    }
                }
            }
            else
                GUILayout.Label($"[找不到id对应配置]");


            if (GUILayout.Button("...", GUILayout.Width(24)))
            {
                ShowMoreMenu(itemAsset);
            }

            if(Attribute.DisplayOption.HasFlag(EDisplayOption.CreateButton))
                if (GUILayout.Button("新建", GUILayout.Width(48)))
                    Create();
            
            if(Attribute.DisplayOption.HasFlag(EDisplayOption.CopyButton))
                if (GUILayout.Button("复制", GUILayout.Width(48)))
                    Copy(itemAsset);

            if(Attribute.DisplayOption.HasFlag(EDisplayOption.SelectButton))
                if (GUILayout.Button("选择", GUILayout.Width(48)))
                    Select();

            EditorGUILayout.EndHorizontal();
            
            if(Attribute.DisplayOption.HasFlag(EDisplayOption.Detail))
                if (itemAsset != null)
                {
                    var key = UniqueDrawerKey.Create(this.Property, this);
                    _enablePropTreeGroup = SirenixEditorGUI.Foldout(_enablePropTreeGroup, "详细");
                    if (_enablePropTreeGroup)
                    {
                        var propertyTree = AvatarConfig.Instance.Editor_GetPropertyTreeByID(itemAsset.ID);
                        if (propertyTree != null)
                        {
                            propertyTree.BeginDraw(true);
                            propertyTree.DrawProperties();
                            if (propertyTree.ApplyChanges())
                            {
                                propertyTree.DelayAction(() =>
                                {
                                    AvatarConfig.Instance.Editor_MaskItemAssetDirty(itemAsset.ID, false);
                                });
                            }

                            propertyTree.EndDraw();
                        }
                    }
                }

            SirenixEditorGUI.EndBox();
        }

        public void ShowMoreMenu(AvatarConfigItemAsset item)
        {
            AvatarConfig.Instance.Editor_ShowMoreMenu(menu =>
            {
                if (!Attribute.DisplayOption.HasFlag(EDisplayOption.SelectButton))
                    menu.AddItem(new GUIContent("选择"), false, Select);
                if (!Attribute.DisplayOption.HasFlag(EDisplayOption.CreateButton))
                    menu.AddItem(new GUIContent("新建"), false, Create);
                if (item != null && !Attribute.DisplayOption.HasFlag(EDisplayOption.CopyButton))
                    menu.AddItem(new GUIContent("复制"), false, () => { Copy(item); });
                if (item != null)
                    menu.AddItem(new GUIContent("删除"), false, () => { AvatarConfig.Instance.Editor_RemoveItem(item.ID); });
            });
        }

        protected void Create()
        {
            AvatarConfig.Instance.Editor_OpenItemCreateWindow((config) =>
            {
                ValueEntry.SmartValue = config.ID;
                ValueEntry.ApplyChanges();
            });
        }
        protected void Copy(AvatarConfigItemAsset itemAsset)
        {
            if (itemAsset != null)
            {
                var errCode = AvatarConfig.Instance.Editor_CopyItemAsset(itemAsset, out var copy);
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
        protected void Select()
        {
            AvatarConfig.Selector.Show((config) =>
            {
                ValueEntry.SmartValue = config.ID;
                ValueEntry.ApplyChanges();
            });
        }
    }
}
#endif
