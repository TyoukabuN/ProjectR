using System;

namespace PJR.Core
{
    /// <summary>
    /// 变量
    /// 这可能只是一个展示大概怎样实现IValue<T>的例子
    /// 因为预先声明声明好_objectValue:object和_value:T的抽象类不方便用Odin来画编辑器
    /// </summary>
    /// <typeparam name="T">变量类型。</typeparam>
    public abstract class Variable<T> : IValue<T>
    {
        [NonSerialized]
        private object _objectValue;
        private T _value;

        public Type ValueType => typeof(T);
        public T TypedValue => _value;
        public object ObjectValue => _objectValue;
    }
}
