using System;
using System.Collections.Generic;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class SequenceRunner : UnitRunner<TrackRunner>
    {
        /// <summary>
        /// The maximum delta time allowed per DriveUpdate call.
        /// Similar to Unity's Time.maximumDeltaTime.
        /// </summary>
        public static float maximumDeltaTime = 0.1f;

        protected GameObject _gameObject;

        public abstract void OnStart();

        public void DriveUpdate(float deltaTime)
        {
            if (deltaTime > maximumDeltaTime)
                deltaTime = maximumDeltaTime;
            OnDriveUpdate(deltaTime);
        }

        protected abstract void OnDriveUpdate(float deltaTime);

        protected float GetSequenceDuration()
        {
            return Sequence?.Duration ?? 0;
        }
        protected double GetTimeScale()
        {
#if UNITY_EDITOR
            // EditMode 下 Time.timeScale 确实是 0
            if (!UnityEditor.EditorApplication.isPlaying)
                return 1.0;
#endif
            return Time.timeScale;
        }
        protected double GetSecondPerFrame() => Utility.GetSecondPerFrame(Sequence?.FrameRateType ?? Define.EFrameRate.Game);
        protected float GetSecondPerFrame_Float() => (float)GetSecondPerFrame();

        protected override void OnClear()
        {
            _gameObject = null;
        }

        public abstract override void Release();
    }
}