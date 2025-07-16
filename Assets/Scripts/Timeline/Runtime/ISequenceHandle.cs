namespace PJR.Timeline
{
    /// <summary>
    /// 可以直接用来控制播放
    /// </summary>
    public interface ISequenceHandle
    {
        double time { get; set; }
        bool Valid { get; }
        SequenceAsset SequenceAsset { get; }
        SequenceDirector Director { get; }
        double ToGlobalTime(double t);
        double ToLocalTime(double t);
        void Release();
    }
}