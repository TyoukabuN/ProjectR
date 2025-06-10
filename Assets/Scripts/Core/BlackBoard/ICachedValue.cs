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
        public bool FromBuffer(IToBufferToken index, bool clearBuffer);
        public bool FromBuffer(int index, uint guid, bool clearBuffer);
        /// <summary>
        /// 将value传到buffer
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ToBuffer(out IToBufferToken token);
        public bool ToBuffer(out int index, out uint guid);

        public interface IToBufferToken
        {
            public uint guid { get; }
            public int index { get; }
            public bool Valid();
            public void Release();
        }
    }
}