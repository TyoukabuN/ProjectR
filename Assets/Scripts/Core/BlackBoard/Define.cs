using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace PJR.BlackBoard
{
    public enum EGetValueApproach
    {
        LocalValue = 0,
        FromBoard,
        ByEvaluation
    }
}

namespace PJR.BlackBoard.CachedValueBoard
{
    public abstract class BoardValueReference
    {
        public abstract Type ValueType { get; }
        [HideInInspector]
        public CacheableValueBoard Board;
        [DisableIf("@true")]
        [LabelWidth(30),HorizontalGroup]
        [HideInInspector]
        public string Key;
        public bool Invalid => Board == null || string.IsNullOrEmpty(Key);
        public static BoardValueReference GetGenericObject(Type type)
        {
            Type genericType = typeof(BoardValueReference<>).MakeGenericType(type);
            return  Activator.CreateInstance(genericType) as BoardValueReference;
        }
        
        #if UNITY_EDITOR

        [HorizontalGroup]
        [ShowInInspector]
        [HideLabel]
        public string Editor_Key
        {
            get => Key;
        }
        #endif
    }

    public class BoardValueReference<T>: BoardValueReference
    {
        public override Type ValueType => typeof(T);
#if UNITY_EDITOR
        [HorizontalGroup]
        [ShowInInspector]
        [HideLabel]
        public T Editor_Value
        {
            get
            {
                T value = default;
                if (Board?.TryGetValue(Key, out value) ?? false)
                    return value;
                return value;
            }
        }
#endif
    }
}
