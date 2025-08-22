using System;
using UnityEngine;
using UnityEngine.Pool;

namespace LS.Game.DataContext
{
    /// <summary>
    /// 相当于一个设计足够冗余的表头
    /// </summary>
    public abstract partial class PropertyBlock : IPropertyBlock
    {
        public class Persistent : PropertyBlock
        {
            public override bool IsTemp => false;
        }

        public class Temp : PropertyBlock
        {
            public override bool IsTemp => true;

            public static Temp Get()
            {
                var temp = GenericPool<Temp>.Get();
                temp.Reset();
                return temp;
            }

            public static Temp Get(PropertyBlock other)
            {
                var temp = GenericPool<Temp>.Get();
                temp.FloatBlock = other.FloatBlock;
                temp.IntBlock = other.IntBlock;
                temp.BoolBlock = other.BoolBlock;
                temp.StringBlock = other.StringBlock;
                return temp;
            }

            public override void Release() => GenericPool<Temp>.Release(this);
        }

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