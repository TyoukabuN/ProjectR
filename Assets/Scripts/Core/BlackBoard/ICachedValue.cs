using System;

namespace PJR.BlackBoard.CachedValueBoard
{
    public interface ICachedValue
    {
        public Type ValueType { get; }
        /// <summary>
        /// 从buffer那获取value
        /// </summary>
        /// <param name="index">buffer索引</param>
        /// <returns></returns>
        public bool FromBuffer(int index);
     
        /// <summary>
        /// 将value传到buffer
        /// </summary>
        /// <param name="index">如果传到buffer成功的话会反一个索引</param>
        /// <returns></returns>
        public bool ToBuffer(out int index);
    }
}