namespace PJR.Core
{
    /// <summary>
    /// 可释放接口
    /// 用在诸如PooledObject,Runner之类的的可释放对象上
    /// </summary>
    public interface IReleasable
    {
        void Release();
    }
}