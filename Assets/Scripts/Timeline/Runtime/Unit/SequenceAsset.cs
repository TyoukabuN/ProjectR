using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public partial class SequenceAsset : SerializedScriptableObject, ISequence
    {
        private bool _runtimeTempInstance = false;
        public bool RuntimeTempInstance => _runtimeTempInstance;
        
        public EFrameRate frameRateType;
        [OdinSerialize]
        private List<Track> _tracks;
        public EFrameRate FrameRateType { get => frameRateType; set => frameRateType = value; }
        public double FrameRate => FrameRateType.FPS();
        public int Frames;
        int ISequence.Frames { get => Frames; }

        // ISequence 接口实现：只读接口列表
        IReadOnlyList<ITrack> ISequence.Tracks => _tracks as IReadOnlyList<ITrack> ?? (_tracks ??= new List<Track>());
        
        public bool Valid
        {
            get
            {
                if (_tracks == null || _tracks.Count <= 0)
                    return false;
                return true;
            }
        }

#if UNITY_EDITOR
        public void Editor_MarkDirty()
        {
            EditorUtility.SetDirty(this);
            if (_tracks == null)
                return;
            for (var i = 0; i < _tracks.Count; i++)
            {
                var track = _tracks[i];
                track.Editor_MarkDirty();
            }
        }
        public int Editor_GetSequenceMaxFrame()
        {
            int maxFrame = 0;
            for (var i = 0; i < _tracks.Count; i++)
            {
                var track = _tracks[i];
                if(track == null)
                    continue;
                if(track.Clips == null || track.Clips.Count <= 0)
                    continue;
                foreach (var clip in track.Clips)
                {
                    if (clip == null)
                        continue;
                    if (clip.EndFrame > maxFrame)
                        maxFrame = clip.EndFrame;
                }
            }

            return maxFrame;
        }
        public void Editor_AddTrack(Track track)
        {
            _tracks ??= new List<Track>();
            _tracks.Add(track);
        }
        public bool Editor_RemoveTrack(Track track)
        {
            if (_tracks == null) return false;
            return _tracks.Remove(track);
        }
        public bool Editor_ContainsTrack(Track track) => _tracks?.Contains(track) ?? false;
        public List<Track> Editor_Tracks => _tracks ??= new List<Track>();
#endif
    }
    public interface ISequence
    {
        public bool RuntimeTempInstance { get; }
        public EFrameRate FrameRateType { get; set; }
        public IReadOnlyList<ITrack> Tracks { get; }
        public bool Valid { get; }
        /// <summary>
        /// 序列总帧数（所有 Clip 中最大的 EndFrame）
        /// </summary>
        public int Frames { get; }
        public SequenceRunner GetRunner(GameObject gameObject);
    }
    public static class ISequenceExtensions
    {
        public static double CalculateDuration(this ISequence sequence)
        {
            if (sequence == null)
                return 0;
            double secondPerFrame = Utility.GetSecondPerFrame(sequence.FrameRateType);
            return sequence.Frames * secondPerFrame;
        }
    }
}
