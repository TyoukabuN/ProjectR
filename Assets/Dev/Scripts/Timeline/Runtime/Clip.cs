using System;

namespace PJR.Timeline
{
    [Serializable]
    public abstract class Clip
    {
        public const string DefaultName = "EmptyName";

        public string clipName;
        public int ClipType;

        public float start;
        public float end;

        public int[] dependencyIDs;

        public virtual string GetDisplayName() => clipName;

        public Clip() { this.clipName = DefaultName; }
        public Clip(string clipName) { this.clipName = clipName; }
    }
}
