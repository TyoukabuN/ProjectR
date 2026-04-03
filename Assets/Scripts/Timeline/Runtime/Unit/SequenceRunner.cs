using System;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class SequenceRunner : UnitRunner
    {
        /// <summary>
        /// The maximum delta time allowed per DriveUpdate call.
        /// Similar to Unity's Time.maximumDeltaTime.
        /// </summary>
        public static float maximumDeltaTime = 0.1f;

        protected abstract ISequence sequence { get; }

        protected GameObject _gameObject;

        public abstract void OnStart();

        public void DriveUpdate(float deltaTime)
        {
            if (deltaTime > maximumDeltaTime)
                deltaTime = maximumDeltaTime;
            OnDriveUpdate(deltaTime);
        }

        protected abstract void OnDriveUpdate(float deltaTime);

        protected double GetTimeScale() => Time.timeScale;
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(sequence?.FrameRateType ?? Define.EFrameRate.Game);
        float GetSecondPerFrame_Float() => (float)GetSecondPerFrame();

        protected override void OnClear()
        {
            _gameObject = null;
        }

        public abstract override void Release();
    }
}