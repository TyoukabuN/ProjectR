using System;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class SequenceRunner : UnitRunner
    {
        protected abstract ISequence sequence { get; }

        protected GameObject _gameObject;

        public abstract void OnStart();
        public abstract void OnUpdate(float deltaTime, bool force = false);

        public virtual void Pause()
        {
            if (IsPaused)
                return;
            runnerState = ERunnerState.Paused;
        }
        
        protected UpdateContext _updateContext;

        protected UpdateContext UpdateContext(double scaledDeltaTime, double unscaledDeltaTime)
        {
            _updateContext.timeScale = GetTimeScale();
            _updateContext.totalTime = TotalTime;

            _updateContext.unscaledDeltaTime = unscaledDeltaTime;
            _updateContext.deltaTime = scaledDeltaTime;

            _updateContext.updateIntervalType = IntervalType.Second;
            _updateContext.gameObject = _gameObject;

            return _updateContext;
        }

        protected UpdateContext UpdateContext(int frame)
        {
            if (frame > 0)
            {
                _updateContext.frameChanged = true;
                _updateContext.totalFrame += frame;
            }

            _updateContext.updateIntervalType = IntervalType.Frame;

            return _updateContext;
        }

        protected double GetTimeScale() => Time.timeScale;
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(sequence?.FrameRateType ?? Define.EFrameRate.Game);
        float GetSecondPerFrame_Float() => (float)GetSecondPerFrame();

        public override void Clear()
        {
            base.Clear();
            _gameObject = null;
        }

        public abstract override void Release();
    }
}