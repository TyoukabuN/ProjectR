using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    [Serializable]
    public class Sequence : ScriptableObject
    {
        public Clip[] clips;
    }
}
