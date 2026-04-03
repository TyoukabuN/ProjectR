using UnityEngine;

namespace PJR.Timeline
{
    /// <summary>
    /// 序列只读访问接口，编辑器和运行时通用
    /// </summary>
    public interface ISequenceHandle
    {
        float Time { get; set; }     // 只读，不允许外部直接设置时间
        bool Valid { get; }
        ISequence Sequence { get; }
        SequenceAsset SequenceAsset { get; }
        Object Object { get; }
        double ToGlobalTime(double t);
        double ToLocalTime(double t);
        void Release();
    }

    /// <summary>
    /// 可控播放接口，仅 PreviewSequenceHandle 实现
    /// 运行时播放控制请直接调用 SequenceDirector 的方法
    /// </summary>
    public interface ISequencePlayableHandle : ISequenceHandle
    {
        SequenceDirector Director { get; }
        bool IsPlaying();
        void Play();
        void Pause();
        void Stop();
        /// <summary>
        /// 跳转到指定时间并强制刷新一帧
        /// </summary>
        void SeekTo(float time);
        void ManualUpdate(float deltaTime);
    }
}