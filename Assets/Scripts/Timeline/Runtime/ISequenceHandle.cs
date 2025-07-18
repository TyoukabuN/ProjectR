using System;

namespace PJR.Timeline
{
    /// <summary>
    /// 可以直接用来控制播放
    /// </summary>
    public interface ISequenceHandle
    {
        float time { get; set; }
        bool Valid { get; }
        SequenceAsset SequenceAsset { get; }
        UnityEngine.Object Object { get; }
        double ToGlobalTime(double t);
        double ToLocalTime(double t);
        void Release();
    }

    public interface ISequencePlayableHandle : ISequenceHandle
    {
        SequenceDirector Director { get; }
        SequenceRunner SequenceRunner { get; }
        bool IsPlaying();
        void Play();
        void Pause();
        void Stop();
    }
}