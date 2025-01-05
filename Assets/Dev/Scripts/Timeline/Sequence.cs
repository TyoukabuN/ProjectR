using System;
using UnityEngine;

namespace PJR.Timeline
{
    [Serializable]
    public class Sequence : ScriptableObject
    {
        public SequenceItem[] items;
        public enum EState
        {
            None = 0,
            Running,
            Done,
        }
        public EState state = 0;
        public virtual void OnStart()
        { 
            state = EState.Running;
        }
        public virtual void OnUpdate(Context context)
        {
            if (state > EState.Running)
                return;
            if (items == null)
            {
                state = EState.Done;
                return;
            }

        }
    }

    public struct Context
    {
        public int frameCount;
        public float deltaTime;
        public float timeScale;
    }
}
