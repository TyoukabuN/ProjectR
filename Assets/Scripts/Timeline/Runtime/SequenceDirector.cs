using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    /// <summary>
    /// 感觉Sequence的运行依赖Director没啥意义，后面感觉可以抽成一个System之类的单例类来统一Tick
    /// </summary>
    public partial class SequenceDirector : SerializedMonoBehaviour, ISequenceHolder
    {
        public bool PlayOnAwake = false;
        
        [SerializeField]
        protected SequenceAsset _sequenceAsset;
        public SequenceAsset SequenceAsset => _sequenceAsset;

        [ShowInInspector,ShowIf("@_runtimeTempSequence!=null")]
        private ISequence _runtimeTempSequence;
        public ISequence Sequence => _sequenceAsset ?? _runtimeTempSequence;

        [NonSerialized] protected SequenceRunner _runner;
        public SequenceRunner Runner => _runner;

        /// <summary>
        /// 直接透传 Runner 的状态，不再维护单独的 EState
        /// </summary>
        public ERunnerState State => _runner?.runnerState ?? ERunnerState.None;

        void Start()
        {
            if (PlayOnAwake)
                GetRunner();
        }

        private void Update()
        {
            ManualUpdate(Time.deltaTime);
        }

        public void ManualUpdate(float deltaTime, bool force = false)
        {
            if (_runner == null)
                return;

            if (_runner.runnerState == ERunnerState.None)
                _runner.OnStart();

            if (_runner.runnerState == ERunnerState.Running || force)
                _runner.OnUpdate(deltaTime, force);

            if (_runner.runnerState == ERunnerState.Diposed)
            {
                _runner.Release();
                _runner = null;
            }
        }

        /// <summary>
        /// 确保 Runner 已就绪（初始化并 OnStart），由 Director 统一负责而非 Handle
        /// </summary>
        public void EnsureRunnerReady()
        {
            if (_runner == null)
                _runner = GetRunner();
            if (_runner != null && _runner.runnerState == ERunnerState.None)
                _runner.OnStart();
        }

        /// <summary>
        /// 跳转到指定时间并强制刷新，不改变播放状态
        /// </summary>
        public void SeekTo(float seekTime)
        {
            EnsureRunnerReady();
            if (_runner == null) return;
            _runner.TotalTime = seekTime;
            var prevState = _runner.runnerState;
            _runner.runnerState = ERunnerState.Running;
            _runner.OnUpdate(0, true);
            if (prevState != ERunnerState.Running)
                _runner.runnerState = prevState;
        }

        [NonSerialized] private RuntimeSequenceHandle _sequenceHandle;
        [NonSerialized] private PreviewSequenceHandle _previewSequenceHandle;
        public ISequenceHandle GetHandle()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return GetPreviewHandle();
#endif
            return GetRuntimeHandle();
        }

        public bool SetRuntimeTempSequence(ISequence sequence)
        {
            if (sequence == null)
                return false;
            _runtimeTempSequence = sequence;
            return true;
        }

        /// <summary>
        /// 获取EditMode下用的Handle
        /// </summary>
        private ISequenceHandle GetPreviewHandle()
        {
            _sequenceHandle?.Release();
            _sequenceHandle = null;
            if (_previewSequenceHandle == null)
                _previewSequenceHandle = PreviewSequenceHandle.Get(this);
            return _previewSequenceHandle;
        }
        
        /// <summary>
        /// 获取Runtime(PlayMode)下用的Handle
        /// </summary>
        private ISequenceHandle GetRuntimeHandle()
        {
            _previewSequenceHandle?.Release();
            _previewSequenceHandle = null;
            if (_sequenceHandle == null)
                _sequenceHandle = RuntimeSequenceHandle.Get(this);
            return _sequenceHandle;
        }

        public SequenceRunner GetRunner()
        {
            if (Sequence == null)
                return null;
            if (_runner == null || _runner.IsDisposed)
                _runner = Sequence.GetRunner(gameObject);
            return _runner;
        }

        public void Stop()
        {
            _runner?.Release();
            _runner = null;
        }
        
        public void Pause()
        {
            _runner?.Pause();
        }

        public void Play()
        {
            if (_runner == null)
            {
                GetRunner();
                return;
            }

            if (_runner.IsRunning)
                return;
            if (_runner.runnerState >= ERunnerState.Done)
                return;
            _runner.runnerState = ERunnerState.Running;
        }
        public void Replay()
        {
            _runner?.Release();
            _runner = null;
            GetRunner();
        }
    }
}