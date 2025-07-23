using System;
using System.Collections.Generic;
using PJR.BlackBoard;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Pool;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace PJR.Core.BlackBoard.CachedValueBoard
{
    [InlineProperty]
    public class CacheableField<T> : ICacheableValue
    {
        protected static bool ExtractBuffer(int index, out BufferUnit<T> unit) => GenericBuffer<T>.instance.ExtractBuffer(index, out unit);
        protected static void ClearBuffer(int index, uint guid) => GenericBuffer<T>.instance.ClearBuffer(index,guid);
        protected bool UsingLocalValue =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.LocalValue;
        protected bool UsingValueFromBoard =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.FromBoard;
        //protected bool UsingValueByEvaluation =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.ByEvaluation;
        protected bool AnyBoardValueReference => _boardValueReference != null;
        public Type ValueType => typeof(T);

        private CacheableValueBoard _boardReference = null;
        public CacheableValueBoard BoardReference => _boardReference;

        public object GetValue() => _localValue;

        /// <summary>
        /// 获取值的方法
        /// 这个Field是怎样获取值的
        /// 0:本地值 1:黑板
        /// </summary>
        [HorizontalGroup("Value",width:20)]
        [GetValueApproach]
        [SerializeField]
        private int _valueGainApproach = 0;
        
        /// <summary>
        /// 本地值
        /// 如果是从本地拿值的话,editor显示这个
        /// </summary>
        [HorizontalGroup("Value/LocalValue"),] 
        [SerializeField] 
        [HideLabel]
        [ShowIf("UsingLocalValue")]
        private T _localValue;

        /// <summary>
        /// 黑板值引用
        /// 如果是从黑板拿值的话,editor显示这个
        /// </summary>
        [HorizontalGroup("Value/BoardValue"),] 
        [OdinSerialize] 
        [HideLabel]
        [InlineProperty]
        [HideReferenceObjectPicker]
        [ShowIf("UsingValueFromBoard")]
        private BoardValueReference<T> _boardValueReference;

        /// <summary>
        /// 获取有可能获取到一个default(T)
        /// 使用TryGetValue代替会知道取值是否是吧
        /// </summary>
        public T Value
        {
            get
            {
                TryGetValue(out T value);
                return value;
            }
        }

        /// <summary>
        /// 尝试取值,失败会返回default(T)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(out T value)
        {
            value = default;
            if (UsingLocalValue)
            {
                value = _localValue; 
                return true;
            }
            if (UsingValueFromBoard)
            {
                if(_boardValueReference == null)
                    return false;
                value = _boardValueReference.Value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 用于给GenericTypeFilter提过类型筛选
        /// 获取方法在TypeExtension.GetTypeFilter()
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypeFilter()
        {
            yield return typeof(CacheableField<>).MakeGenericType(typeof(T));
        }
        
        /// <summary>
        /// 这个方法只会修改本地值
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clearBuffer"></param>
        /// <returns></returns>
        public bool WriteFromBuffer(ICacheableValue.IToBufferToken token, bool clearBuffer)
        {
            if (!token.Valid())
                return false;
            if (!GenericBuffer<T>.instance.TryGetValue(token, out T bufferValue))
                return false;
            _localValue = bufferValue;
            if(clearBuffer)
                ClearBuffer(token.index, token.guid);
            return true;
        }

        /// <summary>
        /// 这个方法只会修改本地值
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clearBuffer"></param>
        /// <returns></returns>
        public bool WriteFromBuffer(int index, uint guid, bool clearBuffer)
        {
            if (!GenericBuffer<T>.instance.TryGetValue(index, guid, out T bufferValue))
                return false;
            _localValue = bufferValue; 
            if(clearBuffer)
                ClearBuffer(index, guid);
            return true;
        }

        public bool CacheToBuffer(out Type type, out int index, out uint guid)
        {
            type = null;
            index = 0;
            guid = 0;
            if (UsingLocalValue)
                return GenericBuffer<T>.instance.TryCacheValue(_localValue, out type, out index, out guid);
            if (UsingValueFromBoard)
            {
                if (_boardValueReference == null)
                    return false;
                return GenericBuffer<T>.instance.TryCacheValue(_boardValueReference.Value, out type, out index, out guid);
            }
            return false;
        }
        
        public bool CacheToBuffer(out ICacheableValue.IToBufferToken token)
        {
            if (!CacheToBuffer(out Type _,out var index, out var guid))
            {
                token = CacheableValue<T>.ToBufferToken.Invalid;
                return false;
            }
            //exist GC.Alloc
            token = new CacheableValue<T>.ToBufferToken(index, guid);
            return true;
        }
        
        public static implicit operator T(CacheableField<T> rhs)
        {
            if (rhs == null)
                return default;
            return rhs.Value;
        }

#if UNITY_EDITOR
        public bool ShowBoardValueReference => UsingValueFromBoard && AnyBoardValueReference;
        public bool ShowBoardValueGainButton => UsingValueFromBoard && !AnyBoardValueReference;
#endif
    }
    
    
#if UNITY_EDITOR
    public static class BoardEditorUtil
    {
        public struct InspectorPropertyRef<T>
        {
            public InspectorProperty InspectorProperty;
            public T Value;
        }
        public static bool FindObjectInChilds<T>(this InspectorProperty property,out List<InspectorPropertyRef<T>> foundProperty, bool includeSelf = true) where T : class
        {
            foundProperty = null;
            var tempsList = ListPool<InspectorPropertyRef<T>>.Get();

            #region Local Functions
            bool IsTypeMatch(InspectorProperty inspectorProperty) => inspectorProperty.ValueEntry.WeakSmartValue is T;
            void TryAdd(InspectorProperty property)
            {
                if (property == null) 
                    return;
                var value = property?.ValueEntry?.WeakSmartValue as T;
                if (value == null) 
                    return;
                tempsList.Add(new()
                {
                    InspectorProperty = property,
                    Value = value,
                });
            }
            #endregion
            
            var rootProperty = property.SerializationRoot;

            if (includeSelf && IsTypeMatch(rootProperty))
                TryAdd(rootProperty);
            foreach (var childProperty in rootProperty.Children.Recurse())
                TryAdd(childProperty);

            if (tempsList.Count <= 0)
            {
                ListPool<InspectorPropertyRef<T>>.Release(tempsList);
                return false;
            }
          
            foundProperty = tempsList;
            return true;
        }
        
    }
#endif

}

