using System;

namespace PJR.Core
{
    public interface IValue
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type ValueType { get; }
        /// <summary>
        /// object类型(boxed)
        /// </summary>
        public object ObjectValue { get; }
    }

    /// <summary>
    /// 泛型量
    /// </summary>
    public interface IValue<T> : IValue
    {
        /// <summary>
        /// 强类型值
        /// </summary>
        public T TypedValue { get; }
    }
}
