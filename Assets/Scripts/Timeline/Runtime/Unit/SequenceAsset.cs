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
        public List<Track> Tracks
        {
            get => _tracks??=new List<Track>(); 
            set => _tracks = value;
        }
        
        public bool Valid
        {
            get
            {
                if (Tracks == null || Tracks.Count <= 0)
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
#endif
    }
    public interface ISequence
    {
        public bool RuntimeTempInstance { get; }
        public EFrameRate FrameRateType { get; set; }
        public List<Track> Tracks { get; set; }
        public bool Valid { get;}
        public SequenceRunner GetRunner(GameObject gameObject);
    }
}
