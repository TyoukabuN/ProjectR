using System;
using System.Collections.Generic;
using UnityEngine;
using static PJR.Timeline.Define;
using PJR.Timeline.Pool;


namespace PJR.Timeline
{
    public class TrackRunner : IDisposable, IErrorRecorder
    {
        public enum EState
        {
            None = 0,
            Running,
            Done,
            Failure,
            Diposed,
        }
        EState _state = 0;
        public EState state => _state;
        public List<ClipRunner> clipRunners => _clipRunners;

        List<ClipRunner> _clipRunners;
        private Track _track;
        private Sequence _sequence;
        Clip2ClipHandleFunc _clip2ClipHandle;

        public double totalTime = 0f;
        double _timeCounter = 0f;
        public bool Invalid => _state >= EState.Diposed || !string.IsNullOrEmpty(Error);

        public TrackRunner() 
        {
            Dispose();
            _state = EState.None;
        }
        public TrackRunner(Sequence sequence, Track track):this(sequence, track, Global.Clip2ClipHandleFunc) { }
        public TrackRunner(Sequence sequence, Track track, Clip2ClipHandleFunc clip2ClipHandle) 
        {
            Init(sequence, track, clip2ClipHandle);
        }
        public virtual void Dispose()
        {
            ForEachClipRunner(DisposeClipRunner);

            if (_clipRunners != null)
            {
                UnityEngine.Pool.CollectionPool<List<ClipRunner>, ClipRunner>.Release(_clipRunners);
                _clipRunners = null;
            }
            _track = null;
            _sequence = null;
            _clip2ClipHandle = null;

            totalTime = 0f;
            _timeCounter = 0f;
            _state = EState.Diposed;
        }

        public virtual bool Init(Sequence sequence, Track track) => Init(sequence, track, Global.Clip2ClipHandleFunc);
        public virtual bool Init(Sequence sequence, Track track, Clip2ClipHandleFunc clip2ClipHandle)
        {
            _sequence = sequence;
            _track = track;
            _clip2ClipHandle = clip2ClipHandle;

            if (_track == null)
            {
                _state = EState.Failure;
                _error = Define.ErrCode_TrackRuner_TrackIsNull;
                return false;
            }
            if (_clip2ClipHandle == null)
            {
                _state = EState.Failure;
                _error = Define.ErrCode_TrackRuner_Clip2ClipHandle;
                return false;
            }

            _clipRunners = UnityEngine.Pool.CollectionPool<List<ClipRunner>, ClipRunner>.Get();
            for (int i = 0; i < _track.clips.Length; i++)
            {
                var clip = _track.clips[i];
                if (clip == null)
                    continue;
                var clipHandle = _clip2ClipHandle.Invoke(clip);
                if (clipHandle == null)
                    continue;

                _clipRunners.Add(clipHandle);
            }

            ForEachClipRunner(InitClipRunner);

            _state = EState.None;
            return true;
        }
        public virtual void OnInit()
        {
        }
        public virtual void OnStart()
        {
            _state = EState.Running;
        }

        public virtual void OnUpdate(UpdateContext context)
        {
            if (_state >= EState.Done)
                return;
            if (clipRunners == null)
            {
                _state = EState.Done;
                return;
            }

            bool allDone = true;
            for (int i = 0; i < clipRunners.Count; i++)
            {
                var clipHandle = clipRunners[i];
                if (!IsClipRunnerUpdatable(clipHandle))
                    continue;

                if (clipHandle.Clip.OutOfRange(context.totalTime))
                {
                    if (clipHandle.Running)
                        clipHandle.OnEnd();
                }
                else
                { 
                    if (clipHandle.WaitingForStart)
                        clipHandle.OnStart(context);
                    if (clipHandle.Running)
                        clipHandle.OnUpdate(context);
                }

                if(clipHandle.state == ClipRunner.EState.Running)
                    allDone = false;
            }

            _state = allDone ? EState.Done : _state;
        }

        void DisposeClipRunner(ClipRunner clipHandle) => clipHandle?.Dispose();
        void InitClipRunner(ClipRunner clipHandle) => clipHandle?.OnInit();
        void ForEachClipRunner(Action<ClipRunner> func)
        {
            if (clipRunners == null || func == null)
                return;
            for (int i = 0; i < clipRunners.Count; i++)
            {
                var clipHandle = clipRunners[i];
                if(clipHandle == null)
                    continue;
                func.Invoke(clipHandle);
            }
        }

        public bool IsClipRunnerUpdatable(ClipRunner clipHandle)
        {
            if (clipHandle == null)
                return false;
            if (clipHandle.state >= ClipRunner.EState.Done)
                return false;
            return true;
        }
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(_sequence?.FrameRateType ?? EFrameRate.Game);


        #region IErrorRecorder Impl
        string _error;
        public string Error => _error;
        public bool AnyError => !string.IsNullOrEmpty(Error);
        #endregion

        #region Pool
        public static TrackRunner Get() => ObjectPool<TrackRunner>.Get();
        public void Pool() => ObjectPool<TrackRunner>.Release(this);
        #endregion
    }
}
