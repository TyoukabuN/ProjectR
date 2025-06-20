using System;

namespace PJR.Core.BlackBoard.CachedValueBoard
{
    public interface ICacheableValue
    {
        public Type ValueType { get; }
        /// <summary>
        /// 从buffer那获取value
        /// </summary>
        /// <param name="index">buffer索引</param>
        /// <returns></returns>
        public bool WriteFromBuffer(IToBufferToken index, bool clearBuffer);
        public bool WriteFromBuffer(int index, uint guid, bool clearBuffer);
        /// <summary>
        /// 将value传到buffer
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CacheToBuffer(out IToBufferToken token);
        public bool CacheToBuffer(out Type type,out int index, out uint guid);

        public object GetValue();
        
        /// <summary>
        /// R/W值只需要知道type:Type ,index:int, guid:uint
        /// 如果需要包含更多的值,需要定义自己的IToBufferToken
        /// 但是有GC.Alloc
        /// </summary>
        public interface IToBufferToken
        {
            public Type ValueType { get; }
            public uint guid { get; }
            public int index { get; }
            public bool Valid();
            public void Release();
        }
    }
}