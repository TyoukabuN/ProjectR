using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.BlackBoard
{
    public enum EGetValueApproach
    {
        LocalValue = 0,
        FromBoard,
        ByEvaluation
    }
}

namespace PJR.Core.BlackBoard.CachedValueBoard
{
    /// <summary>
    /// 黑板值引用
    /// 包含一个需要用Odin序列化的黑板引用Board
    /// 一个黑板值的Key用作取值用
    /// </summary>
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
        public string Editor_Key => Key;
#endif
    }

    /// <summary>
    /// 黑板值得引用(泛型)
    /// BoardValueReference的子类,包含一个泛型的属性Value用于取值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BoardValueReference<T>: BoardValueReference
    {
        public override Type ValueType => typeof(T);
        
        [HorizontalGroup]
        [ShowInInspector]
        [HideLabel]
        public T Value
        {
            get
            {
                T value = default;
                if (Board?.TryGetValue(Key, out value) ?? false)
                    return value;
                return value;
            }
        }
    }
}
