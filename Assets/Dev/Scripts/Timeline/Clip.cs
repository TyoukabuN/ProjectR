using UnityEngine;
using System;

namespace PJR.Timeline
{
    [Serializable]
    public abstract class Clip
    {
        public int clipType;

        public float start;
        public float end;

        public int[] dependencyIDs;
    }
}
