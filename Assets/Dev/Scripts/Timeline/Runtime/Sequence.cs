using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    public class Sequence : ScriptableObject
    {
        public EFrameRate frameRateType;
        public Clip[] clips;
    }
}
