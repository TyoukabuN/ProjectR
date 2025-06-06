using System;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public class Sequence : SequenceScriptableObject, ISequence
    {
        public EFrameRate frameRateType;
        [SerializeField]
        private Track[] tracks = Array.Empty<Track>(); 
        public bool runtimeInstance;
        public EFrameRate FrameRateType { get => frameRateType; set => frameRateType = value; }
        public Track[] Tracks
        {
            get => tracks??=Array.Empty<Track>(); 
            set => tracks = value;
        }
    }
    public interface ISequence
    {
        public EFrameRate FrameRateType { get; set; }
        public Track[] Tracks { get; set; }
    }
}
