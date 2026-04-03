using System;
using PJR.Timeline.Pool;
using UnityEngine;

namespace PJR.Timeline
{
    /// <summary>
    /// Runner 公共基类：统一状态机、状态变更回调、错误记录、时间字段
    /// </summary>
    public abstract class UnitRunner : PoolableObject
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

        private void Internal_OnStateChanged(ERunnerState oldState, ERunnerState newState)
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
                Debug.LogError($"Runner Failure: {error}");
        }

        // 时间计数
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

        private float _totalTime;
        private float _unscaleTotalTime;

        // SubRunner 抽象接口
        protected virtual void ForeachSubRunner(Action<UnitRunner> action) { }

        /// <summary>
        /// 将当前 Runner 及所有子 Runner 设为 Running 状态
        /// </summary>
        public void Play()
        {
            if (IsRunning)
                return;
            if (runnerState >= ERunnerState.Failure)
                return;
            runnerState = ERunnerState.Running;
            ForeachSubRunner(sub => sub.OnPlay());
        }

        protected abstract void OnPlay();

        /// <summary>
        /// 将当前 Runner 及所有子 Runner 设为 Paused 状态
        /// </summary>
        public void Pause()
        {
            if (IsPaused)
                return;
            runnerState = ERunnerState.Paused;
            ForeachSubRunner(sub => sub.OnPause());
        }

        protected abstract void OnPause();

        public void SetRunnerStateRecursive(ERunnerState state)
        {
            runnerState = state;
            ForeachSubRunner(sub => sub.SetRunnerStateRecursive(state));
        }

        public void ResetStateRecursive()
        {
            SetRunnerStateRecursive(ERunnerState.None);
        }

        public sealed override void Clear()
        {
            if (runnerState == ERunnerState.Diposed)
                return;
            // 1. 先递归清理子Runner
            ForeachSubRunner(sub => sub.Clear());
            // 2. 子类清理自身字段
            OnClear();
            // 3. 基类收尾
            runnerState = ERunnerState.Diposed;
            _error = null;
            OnStateChanged = null;
            _totalTime = 0f;
            _unscaleTotalTime = 0f;
        }

        protected virtual void OnClear() { }

        public virtual void OnUpdate(UpdateContext context) { }
    }
}
