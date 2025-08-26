using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PJR.Dev.Game.DataContext
{
    /// <summary>
    /// 相当于一个设计足够冗余的表头
    /// 用于传递数据至<see cref="EffectProperty"/>
    /// <see cref="PropertyBlock"/>定义了如果从<see cref="PropertyBlock"/>中获取数据
    /// </summary>
    public abstract partial class PropertyBlock : IPropertyBlock
    {
        public static readonly string AttemptModifyNonTemplateString = "尝试修改非临时ParameterBlock";

        public abstract bool IsTemp { get; }

        public ValueChunk8<float> FloatBlock
        {
            get => _floatBlock;
            protected set => _floatBlock = value;
        }

        public ValueChunk8<int> IntBlock
        {
            get => _intBlock;
            protected set => _intBlock = value;
        }

        public ValueChunk4<bool> BoolBlock
        {
            get => _boolBlock;
            protected set => _boolBlock = value;
        }
        public ValueChunk4<string> StringBlock
        {
            get => _stringBlock;
            protected set => _stringBlock = value;
        }

        [SerializeField] private ValueChunk8<float> _floatBlock;
        [SerializeField] private ValueChunk8<int> _intBlock;
        [SerializeField] private ValueChunk4<bool> _boolBlock;
        //todo:string会有GC
        [SerializeField] private ValueChunk4<string> _stringBlock;

        public bool CanModify(string logStr = null)
        {
            if (!IsTemp)
            {
                if (!string.IsNullOrEmpty(logStr))
                    Debug.LogError(logStr);
                return false;
            }

            return true;
        }

        public float GetFloat(int index) => _floatBlock.Get(index);
        public int GetInt(int index) => _intBlock.Get(index);
        public bool GetBool(int index) => _boolBlock.Get(index);
        public string GetString(int index) => _stringBlock.Get(index);

        public bool SetFloat(int index, float value, bool force = false)
        {
            if (!force && !CanModify(AttemptModifyNonTemplateString))
                return false;
            _floatBlock.Set(index, value);
            return true;
        }

        public bool SetInt(int index, int value, bool force = false)
        {
            if (!force && !CanModify(AttemptModifyNonTemplateString))
                return false;
            _intBlock.Set(index, value);
            return true;
        }

        public bool SetBool(int index, bool value, bool force = false)
        {
            if (!force && !CanModify(AttemptModifyNonTemplateString))
                return false;
            _boolBlock.Set(index, value);
            return true;
        }
        
        public bool SetString(int index, string value, bool force = false)
        {
            if (!force && !CanModify(AttemptModifyNonTemplateString))
                return false;
            _stringBlock.Set(index, value);
            return true;
        }


        public bool Set<T>(int index, T value, bool force = false)
        {
            if (value is float floatValue)
                return SetFloat(index, floatValue, force);
            if (value is int intValue)
                return SetInt(index, intValue, force);
            if (value is bool boolValue)
                return SetBool(index, boolValue, force);
            if (value is string stringValue)
                return SetString(index, stringValue, force);
            return false;
        }
        
        public T Get<T>(int index)
        {
            if (typeof(T) == typeof(float))
                return Unsafe.As<float, T>(ref Unsafe.AsRef(GetFloat(index)));
            if (typeof(T) == typeof(int))
                return Unsafe.As<int, T>(ref Unsafe.AsRef(GetInt(index)));
            if (typeof(T) == typeof(bool))
                return Unsafe.As<bool, T>(ref Unsafe.AsRef(GetBool(index)));
            if (typeof(T) == typeof(string))
                return Unsafe.As<string, T>(ref Unsafe.AsRef(GetString(index)));
            return default;
        }

        public void Override(PropertyBlock other, bool force = false)
        {
            if (!force && !CanModify(AttemptModifyNonTemplateString))
                return;
            this.FloatBlock = other.FloatBlock;
            this.IntBlock = other.IntBlock;
            this.BoolBlock = other.BoolBlock;
            this.StringBlock = other.StringBlock;
        }

        public void Reset(bool force = false)
        {
            if (!force && !CanModify(AttemptModifyNonTemplateString))
                return;
            FloatBlock = ValueChunk8<float>.Empty;
            IntBlock = ValueChunk8<int>.Empty;
            BoolBlock = ValueChunk4<bool>.Empty;
            StringBlock = ValueChunk4<string>.Empty;
        }

        void IDisposable.Dispose() => Release();

        public virtual void Release()
        {
        }
    }
}