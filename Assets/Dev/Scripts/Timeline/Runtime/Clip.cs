using System;

namespace PJR.Timeline
{
    [Serializable]
    public abstract class Clip
    {
        public int ClipType;

        public float start;
        public float end;

        public int[] dependencyIDs;
    }
}
