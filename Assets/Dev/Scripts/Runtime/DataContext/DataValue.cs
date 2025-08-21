using System;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Dev.Game.DataContext
{
    /// <summary>
    /// DataValue
    /// 用int,float,bool来描述所有DataType的值
    /// 用float来存值,并模拟int,bool
    /// 类型使用type:int来记录0:float,1:int,2:bool
    /// </summary>
    [HideLabel]
    public struct DataValue : IDataValue, IEquatable<DataValue>
    {
        public enum EValueType
        {
            Float = 0,
            Int = 1,
            Bool = 2,
            Length
        }
        public static implicit operator float(DataValue value)
            => value.FloatValue;
        public static implicit operator int(DataValue value)
            => value.IntValue;
        public static implicit operator bool(DataValue value)
            => value.BoolValue;

        public static implicit operator DataValue(float value)
        {
            return new DataValue
            {
                _valueType = (int)EValueType.Float,
                _floatValue = value
            };
        }
        public static implicit operator DataValue(int value)
        {
            return new DataValue
            {
                _valueType = (int)EValueType.Int,
                _floatValue = value
            };
        }
        public static implicit operator DataValue(bool value)
        {
            return new DataValue
            {
                _valueType = (int)EValueType.Bool,
                _floatValue = value ? 1f : 0f
            };
        }

        // [HorizontalGroup("Layout",width:160)]
        // [SerializeField,HideLabel]
        // private DataType _dataType;
        
        [HorizontalGroup("Layout",width:20)]
        [GetValueApproach,SerializeField,HideLabel]
        private int _valueType;
        
        [HorizontalGroup("Layout/Value"), ShowInInspector, ShowIf("IsFloat"),HideLabel]
        public float FloatValue
        {
            get => IsFloat ? _floatValue : 0f;
            set => _floatValue = value;
        }
        [HorizontalGroup("Layout/Value"), ShowInInspector, ShowIf("IsInt"),HideLabel]
        public int IntValue
        {
            get => IsInt ? (int)_floatValue : 0;
            set => _floatValue = value;
        }
        [HorizontalGroup("Layout/Value"), ShowInInspector, ShowIf("IsBool"),HideLabel]
        public bool BoolValue
        {
            get => IsBool && _floatValue > 0;
            set => _floatValue = value ? 1 : 0;
        }
        float IDataValue.Value => FloatValue;
        public bool IsFloat => _valueType == 0;
        public bool IsInt => _valueType == 1;
        public bool IsBool => _valueType == 2;
        
        [SerializeField,ShowIf("@false")]
        private float _floatValue;

        public bool IsValueTypeMatch(DataValue other)
            => _valueType == other._valueType;

        public static DataValue operator +(DataValue lhs, DataValue rhs)
            => lhs._floatValue + rhs._floatValue;
        public static DataValue operator -(DataValue lhs, DataValue rhs)
            => lhs._floatValue - rhs._floatValue;
        public static DataValue operator *(DataValue lhs, DataValue rhs)
            => lhs._floatValue * rhs._floatValue;
        public static DataValue operator /(DataValue lhs, DataValue rhs)
            => lhs._floatValue / rhs._floatValue;
        public static bool operator ==(DataValue lhs, DataValue rhs)
            => Mathf.Approximately(lhs._floatValue, rhs._floatValue);
        public static bool operator !=(DataValue lhs, DataValue rhs) 
            => !(lhs == rhs);
        public static DataValue operator ++(DataValue lhs)
            => lhs._floatValue++;

        public override string ToString()
        {
            if(IsBool)
                return BoolValue ? Boolean.TrueString : Boolean.FalseString;
            if (IsInt)
                return IntValue.ToString();
            return FloatValue.ToString(CultureInfo.InvariantCulture);
        }
        public bool Equals(DataValue other)
            => Mathf.Approximately(this._floatValue, other._floatValue);
        public override bool Equals(object obj)
            => obj is DataValue other && Equals(other);
        public override int GetHashCode()
            => HashCode.Combine(_valueType, _floatValue);
        
        private void AsFloat() => _valueType = (int)EValueType.Float;
        private void AsInt() => _valueType = (int)EValueType.Int;
        private void AsBool() => _valueType = (int)EValueType.Bool;
        
        
    }
}