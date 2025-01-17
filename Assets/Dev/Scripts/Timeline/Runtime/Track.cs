using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    public class Track : ScriptableObject
    {
        public Clip[] clips;
    }
}
