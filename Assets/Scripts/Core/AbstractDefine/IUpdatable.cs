namespace PJR
{
    /// <summary>
    /// 可更新接口
    /// 可能会用在后续自定义的组件上, 或者是可能的UpdateManager的单元对象上
    /// </summary>
    public interface IUpdatable
    {
        void Update(IUpdateContext baseUpdateContext);
    }
    /// <summary>
    /// 更新上下问接口
    /// </summary>
    public interface IUpdateContext
    {
        /// <summary>
        /// 基础的DeltaTime,或者是ElapseTime,FrameInterval的概念相同
        /// </summary>
        float DeltaTime { get; }
        float UnscaleDeltaTime { get; }
    }
    /// <summary>
    /// 通用的更新上下文
    /// </summary>
    public struct CommonUpdateContext : IUpdateContext
    {
        public float DeltaTime { get; set; }
        public float UnscaleDeltaTime { get; set; }
    }
}

