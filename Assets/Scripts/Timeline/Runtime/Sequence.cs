using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    public class Sequence : ISequence
    {
        public EFrameRate frameRateType;
        [SerializeReference]
        public Track[] tracks;

        public bool runtimeInstance;

        public EFrameRate FrameRateType { get => frameRateType; set => frameRateType = value; }
        public Track[] Tracks { get => tracks; set => tracks = value; }
    }
    public interface ISequence
    {
        public EFrameRate FrameRateType { get; set; }
        public Track[] Tracks { get; set; }
    }
}
