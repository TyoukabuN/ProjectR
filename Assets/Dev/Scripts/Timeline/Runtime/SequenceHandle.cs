using System;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Events;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    public class SequenceHandle : ScriptableObject
    {
        public enum EState
        {
            None = 0,
            Running,
            Done,
            Failure,
        }
        public EState state = 0;
        public List<ClipHandle> clipHandles => _clipHandles;
        public float totalTime = 0f;

        List<ClipHandle> _clipHandles;
        private Sequence _sequence;
        Clip2ClipHandleFunc _clip2ClipHandle;

        public SequenceHandle(Sequence sequence):this(sequence, Global.Clip2ClipHandleFunc) { }
        public SequenceHandle(Sequence sequence, Clip2ClipHandleFunc clip2ClipHandle) 
        {
            _sequence = sequence;
            if (sequence != null)
            { 
                state = EState.Failure;
                return;
            }
            if (clip2ClipHandle == null)
            {
                state = EState.Failure;
                return;
            }
            _clip2ClipHandle = clip2ClipHandle;

            _clipHandles = new List<ClipHandle>(sequence.clips.Length);
            for (int i = 0; i < sequence.clips.Length; i++) 
            { 
                var clip = sequence.clips[i];
                if (clip == null)
                    continue;
                var clipHandle = _clip2ClipHandle.Invoke(clip);
                if (clipHandle == null)
                    continue;

                _clipHandles.Add(clipHandle);
            }

            ForEachClipHandle(InitClipHandle);
        }
        public virtual void OnStart()
        {
            state = EState.Running;
        }

        public virtual void OnUpdate()
        {
            if (state >= EState.Done)
                return;

            totalTime += Time.deltaTime;
            var context = UpdateContext();

            if (clipHandles == null)
            {
                state = EState.Done;
                return;
            }
            bool allDone = true;
            for (int i = 0; i < clipHandles.Count; i++)
            {
                var clipHandle = clipHandles[i];
                if (!IsClipHandleUpdatable(clipHandle))
                    continue;

                if (clipHandle.WaitingForStart)
                    if (IsClipInPlayRange(clipHandle))
                        clipHandle.OnStart();

                if (clipHandle.Running)
                    clipHandle.OnUpdate(context);

                if(clipHandle.state < ClipHandle.EState.Done)
                    allDone = false;
            }

            if (allDone)
            {
                state = EState.Done;
            }
        }

        public virtual void OnDispose() 
        {
            ForEachClipHandle(DisposeClipHandle);
        }

        UpdateContext _updateContext;
        UpdateContext UpdateContext()
        {
            _updateContext.frameCount = Time.frameCount;
            _updateContext.timeScale = Time.timeScale;
            _updateContext.totalTime = totalTime;
            _updateContext.deltaTime = Time.deltaTime;
            return _updateContext;
        }


        void DisposeClipHandle(ClipHandle clipHandle) => clipHandle?.OnDispose();
        void InitClipHandle(ClipHandle clipHandle) => clipHandle?.OnInit();

        void ForEachClipHandle(Action<ClipHandle> func)
        {
            if (clipHandles == null || func == null)
                return;
            for (int i = 0; i < clipHandles.Count; i++)
            {
                var clipHandle = clipHandles[i];
                if(clipHandle == null)
                    continue;
                func.Invoke(clipHandle);
            }
        }

        public bool IsClipHandleUpdatable(ClipHandle clipHandle)
        {
            if (clipHandle == null)
                return false;
            if (clipHandle.state >= ClipHandle.EState.Done)
                return false;
            return true;
        }
        public bool IsClipInPlayRange(ClipHandle clipHandle)
        {
            if (clipHandle == null)
                return false;
            if (totalTime < clipHandle.Clip.start || totalTime > clipHandle.Clip.end)
                return false;
            return true;
        }
    }
    
    public struct UpdateContext
    {
        public int frameCount;
        public float timeScale;
        public float totalTime;

        public float deltaTime;
    }
}
