#region Modified By Hunter (jb) -- 2022年8月22日

namespace Sirenix.Serialization
{
    //搬一下fs的机制
#pragma warning disable
    /// <summary>
    /// 仅当Odin序列化才会用到的回调
    /// </summary>
    public interface IOdinSerializationCollector
    {
        void OnPush();
        void OnCollect(IOdinSerializationCollectable child);
        void OnPop();
    }

    public interface IOdinSerializationCollectable
    {
    }
}

#endregion
