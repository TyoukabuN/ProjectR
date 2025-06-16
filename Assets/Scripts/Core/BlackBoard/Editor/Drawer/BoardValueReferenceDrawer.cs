using System.Linq;
using PJR.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR.BlackBoard.Editor.Drawers
{
    public class BoardValueReferenceDrawer : OdinValueDrawer<BoardValueReference>
    {
        private IPropertyValueEntry<BoardValueReference> valueEntry;
        protected override void Initialize()
        {
            base.Initialize();
            valueEntry = this.Property.TryGetTypedValueEntry<BoardValueReference>();
        }

        private bool _doReChoose = false;
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (valueEntry == null)
            {
                CallNextDrawer(label);
                return;
            }
            
            if (!(valueEntry.SmartValue?.Invalid ?? true))
            {
                GUILayout.BeginHorizontal();
                CallNextDrawer(label);
                if(GUILayout.Button("...",GUIStyles.MiniRichTextButton,GUILayout.Width(22)))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("重新选择"), false, ()=>_doReChoose = true);
                    menu.AddItem(new GUIContent("清空"), false, () =>
                    {
                        valueEntry.SmartValue.Key = null;
                        valueEntry.SmartValue.Board = null;
                        valueEntry.Property.MarkSerializationRootDirty();
                    });
                    menu.ShowAsContext();
                }
                if (_doReChoose)
                {
                    _doReChoose = false;
                    SelectContextBoardValue();
                }

                GUILayout.EndHorizontal();
                return;
            }
            if (GUILayout.Button("获取黑板值引用", GUIStyles.MiniRichTextButton))
            {
                SelectContextBoardValue();
            }
        }

        private void SelectContextBoardValue()
        {
            if (this.Property.FindObjectInChilds<CacheableValueBoard>(out var references))
            {
                var menu = new GenericMenu();
                for (var i =0; i <references.Count; i++)
                {
                    var reference = references[i];
                    var board = reference.Value;

                    var keyValuePairs = board.FindKeyValuesOfType(valueEntry.SmartValue.ValueType);
                    if (keyValuePairs == null)
                        continue;

                    for (var j = keyValuePairs.Length - 1; j >= 0; j--)
                    {
                        var keyValuePair = keyValuePairs[j];
                        var keyName = keyValuePair.Key;
                        var valueType = keyValuePair.Value.ValueType;

                        GUIContent guiContent = null;
                        LabelTextAttribute labelTextAttr = references[i].InspectorProperty.Attributes
                            .FirstOrDefault(a => a is LabelTextAttribute) as LabelTextAttribute;
                        if(labelTextAttr != null)
                            guiContent = new GUIContent($"{labelTextAttr.Text}/{keyName}:[{valueType}]");    
                        else
                            guiContent = new GUIContent($"{references[i].InspectorProperty.Path}/{keyName}:[{valueType}]");    
                        
                        menu.AddItem(guiContent, false, () =>
                        {
                            var temp = BoardValueReference.GetGenericObject(valueType);
                            temp.Key = keyValuePair.Key;
                            temp.Board = board;
                            valueEntry.SmartValue = temp;
                        });
                    }
                }

                menu?.ShowAsContext();
            }
            else
            {
                Debug.Log($"没有找到黑板引用");
            }
        }
    }
}