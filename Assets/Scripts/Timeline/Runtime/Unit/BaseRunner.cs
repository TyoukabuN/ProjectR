using System;
using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    /// <summary>
    /// Runner 公共基类：统一状态机、状态变更回调、错误记录
    /// </summary>
    public abstract class BaseRunner : PoolableObject
    {
        private ERunnerState _runnerState;

        public virtual ERunnerState runnerState
        {
            get => _runnerState;
            set
            {
                if (_runnerState == value)
                    return;
                var old = _runnerState;
                _runnerState = value;
                Internal_OnStateChanged(old, _runnerState);
            }
        }

        public bool IsRunning       => runnerState == ERunnerState.Running;
        public bool IsPaused        => runnerState == ERunnerState.Paused;
        public bool IsDone          => runnerState == ERunnerState.Done;
        public bool IsDisposed      => runnerState == ERunnerState.Diposed;
        public bool IsFailure       => runnerState == ERunnerState.Failure;
        public bool WaitingForStart => runnerState == ERunnerState.None;

        public Action<ERunnerState, ERunnerState> OnStateChanged;

        protected virtual void Internal_OnStateChanged(ERunnerState oldState, ERunnerState newState)
        {
            OnStateChanged?.Invoke(oldState, newState);
            if (newState == ERunnerState.Done)
                Internal_OnDone();
        }

        protected virtual void Internal_OnDone() { }

        // 错误记录
        private string _error;
        public string Error => _error;
        public bool AnyError => !string.IsNullOrEmpty(_error);

        public void AsFailure(string error = null)
        {
            _error = error;
            runnerState = ERunnerState.Failure;
            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError($"Runner Failure: {error}");
        }

        public override void Clear()
        {
            if (runnerState == ERunnerState.Diposed)
                return;
            runnerState = ERunnerState.Diposed;
            _error = null;
            OnStateChanged = null;
        }
    }
}
