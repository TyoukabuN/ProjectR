using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public class SequenceAsset : ScriptableObject
    {
        [SerializeReference]
        public Sequence Sequence;
    }
}
