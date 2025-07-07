using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public class Sequence : SerializedScriptableObject, ISequence
    {
        public EFrameRate frameRateType;
        [OdinSerialize]
        private List<Track> _tracks;
        [DisableIf("@true")]
        public bool runtimeInstance;
        public EFrameRate FrameRateType { get => frameRateType; set => frameRateType = value; }
        public List<Track> Tracks
        {
            get => _tracks??=new List<Track>(); 
            set => _tracks = value;
        }
        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
            if (_tracks == null)
                return;
            for (var i = 0; i < _tracks.Count; i++)
            {
                var track = _tracks[i];
                track.MarkDirty();
            }
        }
    }
    public interface ISequence
    {
        public EFrameRate FrameRateType { get; set; }
        public List<Track> Tracks { get; set; }
    }
}
