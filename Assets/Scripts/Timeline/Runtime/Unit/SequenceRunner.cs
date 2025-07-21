using System;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    public abstract class SequenceRunner : PoolableObject
    {
        public enum EState
        {
            None = 0,
            Running,
            Paused,
            Done,
            Failure,
            Diposed,
        }
        public virtual EState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;
                Internal_OnStateChanged(_state, value);
                _state = value;
                if (_state == EState.Done)
                    Internal_OnDone();
            }
        }
        
        protected EState _state;
        public bool IsRunning => State == EState.Running;
        public bool IsPaused => State == EState.Paused;
        public bool IsDone => State == EState.Done;
        public bool IsDisposed => State == EState.Diposed;

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
        public Action<EState, EState> OnStateChanged;
        
        public abstract void OnStart();
        public abstract void OnUpdate(float deltaTime, bool force = false);
        protected virtual void Internal_OnStateChanged(EState oldState, EState newState) 
            => OnStateChanged?.Invoke(oldState, newState);
        protected virtual void Internal_OnDone() => Clear();
        public virtual void Pause()
        {
            if (IsPaused)
                return;
            State = EState.Paused;
        }

        public override void Clear()
        {
            if (_state == EState.Diposed)
                return;
            State = EState.Diposed;
            _totalTime = 0f;
            _unscaleTotalTime = 0f;
            OnStateChanged = null;
        }
        public abstract override void Release();
    }
}