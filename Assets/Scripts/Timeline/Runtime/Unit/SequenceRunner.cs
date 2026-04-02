using System;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class SequenceRunner : BaseRunner
    {
        protected abstract ISequence sequence { get; }

        protected GameObject _gameObject;

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
        
        public abstract void OnStart();
        public abstract void OnUpdate(float deltaTime, bool force = false);

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
            base.Clear();
            _totalTime = 0f;
            _unscaleTotalTime = 0f;
            _gameObject = null;
        }

        public abstract override void Release();
    }
}