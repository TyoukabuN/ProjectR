using UnityEngine;
using System;

namespace PJR.Timeline
{
    [Serializable]
    public abstract class Clip
    {
        public float start;
        public float end;

        public abstract int ClipType { get; }

        //public int[] dependencyIDs;

    }
}
