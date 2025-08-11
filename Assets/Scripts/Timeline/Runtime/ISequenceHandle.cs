using UnityEngine;

namespace PJR.Timeline
{
    /// <summary>
    /// 可以直接用来控制播放
    /// </summary>
    public interface ISequenceHandle
    {
        float time { get; set; }
        bool Valid { get; }
        ISequence Sequence { get; }
        SequenceAsset SequenceAsset { get; }
        Object Object { get; }
        double ToGlobalTime(double t);
        double ToLocalTime(double t);
        void Release();
    }

    public interface ISequencePlayableHandle : ISequenceHandle
    {
        SequenceDirector Director { get; }
        SequenceRunner Runner { get; }
        bool IsPlaying();
        void Play();
        void Pause();
        void Stop();
    }
}