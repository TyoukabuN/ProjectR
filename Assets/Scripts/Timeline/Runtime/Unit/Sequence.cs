using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
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
    }
    public interface ISequence
    {
        public EFrameRate FrameRateType { get; set; }
        public List<Track> Tracks { get; set; }
    }
}
