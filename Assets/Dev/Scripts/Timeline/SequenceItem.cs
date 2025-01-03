using UnityEngine;
using System;

namespace PJR.Timeline
{
    [Serializable]
    public abstract class SequenceItem
    {
        public SequenceObject sequence;

        public int[] dependencyIDs;
        public enum EState
        {
            None = 0,
            Running,
            Done,
        }
        public EState state = 0;
        public abstract void OnStart(Context context);
        public abstract void OnUpdate(Context context);
        public virtual void OnDispose() { }
        public virtual bool CanStart() => true;
    }
}
