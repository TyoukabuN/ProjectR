using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public class Sequence : ScriptableObject
    {
        public EFrameRate frameRateType;
        public Track[] tracks;

        public bool runtimeInstance;
    }
}
