using System;
using System.Collections.Generic;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.BlackBoard.CachedValueBoard
{
    [InlineProperty]
    public class CacheableField<T> : ICacheableValue
    {
        public static bool ExtractBuffer(int index, out BufferUnit<T> unit) => VariableBuffer<T>.instance.ExtractBuffer(index, out unit);
        public static void ClearBuffer(int index, uint guid) => VariableBuffer<T>.instance.ClearBuffer(index,guid);
        public bool UsingLocalValue =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.LocalValue;
        public bool UsingValueFromBoard =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.FromBoard;
        //public bool UsingValueByEvaluation =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.ByEvaluation;
        public bool AnyBoardValueReference => _boardValueReference != null;
        public Type ValueType => typeof(T);

        private CacheableValueBoard _boardReference = null;
        public CacheableValueBoard BoardReference => _boardReference;

        public object GetValue() => _localValue;

        [HorizontalGroup("Value",width:20)]
        [GetValueApproach]
        [SerializeField]
        private int _valueGainApproach = 0;
        
        [HorizontalGroup("Value/LocalValue"),] 
        [SerializeField] 
        [HideLabel]
        [ShowIf("UsingLocalValue")]
        private T _localValue;

        [HorizontalGroup("Value/BoardValue"),] 
        [OdinSerialize] 
        [HideLabel]
        [InlineProperty]
        [HideReferenceObjectPicker]
        [ShowIf("UsingValueFromBoard")]
        private BoardValueReference<T> _boardValueReference;

        public void WriteFromBoard(CacheableValueBoard board)
        {
        }

        public static IEnumerable<Type> GetFiTypeFilter()
        {
            yield return typeof(CacheableField<>).MakeGenericType(typeof(T));
        }
        public bool WriteFromBuffer(ICacheableValue.IToBufferToken token, bool clearBuffer)
        {
            if (!token.Valid())
                return false;
            if (!VariableBuffer<T>.instance.TryGetValue(token, out T bufferValue))
                return false;
            _localValue = bufferValue;
            if(clearBuffer)
                ClearBuffer(token.index, token.guid);
            return true;
        }

        public bool WriteFromBuffer(int index, uint guid, bool clearBuffer)
        {
            if (!VariableBuffer<T>.instance.TryGetValue(index, guid, out T bufferValue))
                return false;
            _localValue = bufferValue; 
            if(clearBuffer)
                ClearBuffer(index, guid);
            return true;
        }

        public bool CacheToBuffer(out Type type, out int index, out uint guid)
            => VariableBuffer<T>.instance.TryCacheValue(_localValue, out type, out index, out guid);
        
        public bool CacheToBuffer(out ICacheableValue.IToBufferToken token)
        {
            if (!CacheToBuffer(out Type type,out var index, out var guid))
            {
                token = CacheableValue<T>.ToBufferToken.Invalid;
                return false;
            }
            //exist GC.Alloc
            token = new CacheableValue<T>.ToBufferToken(index, guid);
            return true;
        }
        
#if UNITY_EDITOR
        public bool ShowBoardValueReference => UsingValueFromBoard && AnyBoardValueReference;
        public bool ShowBoardValueGainButton => UsingValueFromBoard && !AnyBoardValueReference;
        
        // [HorizontalGroup("Value/BoardValue")] 
        // [Button("获取黑板值")]
        // [ShowIf("ShowBoardValueGainButton")]
        // public void Editor_FindBoard()
        // {
        // }
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

