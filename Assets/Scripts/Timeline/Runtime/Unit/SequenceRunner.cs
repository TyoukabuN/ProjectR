using System;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class SequenceRunner : PoolableObject
    {
        protected abstract ISequence sequence { get; }

        protected GameObject _gameObject;
        public virtual ERunnerState runnerState
        {
            get => RunnerState;
            set
            {
                if (RunnerState == value)
                    return;
                Internal_OnStateChanged(RunnerState, value);
                RunnerState = value;
                if (RunnerState == ERunnerState.Done)
                    Internal_OnDone();
            }
        }
        
        protected ERunnerState RunnerState;
        public bool IsRunning => runnerState == ERunnerState.Running;
        public bool IsPaused => runnerState == ERunnerState.Paused;
        public bool IsDone => runnerState == ERunnerState.Done;
        public bool IsDisposed => runnerState == ERunnerState.Diposed;

        public float TotalTime
        {
            get => _totalTime;
            set => _totalTime = value;
        }

        public float UnscaleTotalTime
        {
            get => _unscaleTotalTime;
            set => _unscaleTotalTime = value;
        }

        float _totalTime = 0f;
        float _unscaleTotalTime = 0f;
        public Action<ERunnerState, ERunnerState> OnStateChanged;
        
        public abstract void OnStart();
        public abstract void OnUpdate(float deltaTime, bool force = false);
        protected virtual void Internal_OnStateChanged(ERunnerState oldRunnerState, ERunnerState newRunnerState) 
            => OnStateChanged?.Invoke(oldRunnerState, newRunnerState);
        protected virtual void Internal_OnDone() => Clear();
        public virtual void Pause()
        {
            if (IsPaused)
                return;
            runnerState = ERunnerState.Paused;
        }
        
        protected Define.UpdateContext _updateContext;

        protected Define.UpdateContext UpdateContext(double scaledDeltaTime, double unscaledDeltaTime)
        {
            _updateContext.timeScale = GetTimeScale();
            _updateContext.totalTime = _totalTime;

            _updateContext.unscaledDeltaTime = unscaledDeltaTime;
            _updateContext.deltaTime = scaledDeltaTime;

            _updateContext.updateIntervalType = Define.IntervalType.Second;
            _updateContext.gameObject = _gameObject;

            return _updateContext;
        }

        protected Define.UpdateContext UpdateContext(int frame)
        {
            if (frame > 0)
            {
                _updateContext.frameChanged = true;
                _updateContext.totalFrame += frame;
            }

            _updateContext.updateIntervalType = Define.IntervalType.Frame;

            return _updateContext;
        }

        protected double GetTimeScale() => Time.timeScale;
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(sequence?.FrameRateType ?? Define.EFrameRate.Game);
        float GetSecondPerFrame_Float() => (float)GetSecondPerFrame();
        public override void Clear()
        {
            if (RunnerState == ERunnerState.Diposed)
                return;
            runnerState = ERunnerState.Diposed;
            _totalTime = 0f;
            _unscaleTotalTime = 0f;
            _gameObject = null;
            OnStateChanged = null;
        }
        public abstract override void Release();
    }
}