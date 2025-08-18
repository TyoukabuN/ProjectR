using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

public class AffixTest : SerializedMonoBehaviour
{
    [NonSerialized,OdinSerialize]
    public DataUnit TestUnit;
    
    [NonSerialized,OdinSerialize]
    public PropertyPackege TestPackege;
    
    public class PropertyPackege
    {
        [SerializeField,ShowInInspector]
        private Dictionary<DataType, DataUnit> _dataMap = new();
        public PropertyPackege()
        {
        }
    }
    
    public interface IPropertyUnit
    {
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
        public bool BoolValue { get; set; }
    }

    /// <summary>
    /// DataUnit
    /// 用int,float,bool来描述所有DataType
    /// bool可以可以用int来代替(true:1)(false:0),但是已经使用了FieldOffset了,不用省了
    /// 类型使用type:byte来记录0:bool,1:int,2:float
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct DataUnit : IPropertyUnit
    {
        public enum EType
        {
            Float = 0,
            Int,
            Bool,
            Length
        }
        
        public float FloatValue
        {
            get => IsFloat ? _floatValue : 0f;
            set
            {
                _floatValue = value;
                AsFloat(); 
            }
        }
        public int IntValue
        {
            get => IsInt ? _intValue : 0;
            set
            {
                _intValue = value;
                AsInt(); 
            }
        }
        public bool BoolValue
        {
            get => IsBool && _boolValue;
            set
            {
                _boolValue = value;
                AsBool();
            }
        }
        public bool IsFloat => _type == 0;
        public bool IsInt => _type == 1;
        public bool IsBool => _type == 2;
        
        [HorizontalGroup("Value",width:32)]
        [GetValueApproach,SerializeField,HideLabel]
        [FieldOffset(0)]
        [OnValueChanged("Editor_OnTypeChange")]
        private int _type;
        [HorizontalGroup("Value/Float")]
        [SerializeField,HideLabel,ShowIf("IsFloat")]
        [FieldOffset(4)]
        private float _floatValue;
        [HorizontalGroup("Value/Int")]
        [SerializeField,HideLabel,ShowIf("IsInt")]
        [FieldOffset(4)]
        private int _intValue;
        [HorizontalGroup("Value/Bool")]
        [SerializeField,HideLabel,ShowIf("IsBool")]
        [FieldOffset(4)]
        private bool _boolValue;
        
        private void AsFloat() => _type = 0;
        private void AsInt() => _type = 1;
        private void AsBool() => _type = 2;

#if UNITY_EDITOR
        private void Editor_OnTypeChange()
        {
            _floatValue = 0f; 
            _intValue = 0;
            _boolValue = false;
        }
#endif
    }

    public void Test()
    {
    }
    
    public class DataUnitDrawer : OdinValueDrawer<DataUnit>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }
    }
    public class GetValueApproachAttribute : Attribute
    {
    }
    public class GetValueApproachAttributeDrawer : OdinAttributeDrawer<GetValueApproachAttribute>
    {
        public const string FloatValueColor = "#98FB98";
        public const string IntValueColor = "#ADD8E6";
        public const string BoolValueColor = "#FFE100";
        
        private IPropertyValueEntry<int> valueEntry;
        private static GUIContent FloatContent => floatContent ??= new GUIContent($"<color={FloatValueColor}>Float</color>", "Float值. 点击切换为数据类型");
        private static GUIContent IntContent => intContent ??= new GUIContent($"<color={IntValueColor}>Int</color>", "Int值. 点击切换为数据类型");
        private static GUIContent BoolContent => boolContent ??= new GUIContent($"<color={BoolValueColor}>Bool</color>", "Bool值. 点击切换为数据类型");
        
        private static GUIContent floatContent;
        private static GUIContent intContent;
        private static GUIContent boolContent;
        

        protected override void Initialize()
        {
            base.Initialize();
            valueEntry = this.Property.TryGetTypedValueEntry<int>();
        }

        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ValueEntry != null && property.ValueEntry.TypeOfValue == typeof(int);
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (valueEntry == null) return;
            GUIContent c;
            var approach = valueEntry.SmartValue;

            if (approach == 0)
                c = FloatContent;
            else if (approach == 1)
                c = IntContent;
            else
                c = BoolContent;

            if (GUILayout.Button(c, GUIStyles.MiniRichTextButton, GUILayout.Width(32f)))
            {
                valueEntry.SmartValue++;
                valueEntry.SmartValue %= (int)DataUnit.EType.Length;
            }
        }
    }
    

    public static class GUIStyles
    {
        private static GUIStyle _minirichTextButton;

        public static GUIStyle MiniRichTextButton
        {
            get
            {
                if (_minirichTextButton == null)
                {
                    _minirichTextButton = new GUIStyle(EditorStyles.miniButton);
                    _minirichTextButton.richText = true;
                    _minirichTextButton.padding = new RectOffset(0, 0, 0, 0);
                    _minirichTextButton.alignment = TextAnchor.MiddleCenter;
                }

                return _minirichTextButton;
            }
        }
    }
    public enum DataType : uint
    {
        BasicAtkSpeed = 5000,
        DynaAtkSpeed,
        DynaAtkSpeedRate,
        CurrentAtkSpeed,
    }

}
