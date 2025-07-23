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
        public enum EState
        {
            None,
            WaitingForStart,
            Running,
            Paused,
            Failure,
            Done,
        }
        [NonSerialized]
        private EState _state = EState.None;
        public EState State => _state;
        
        public bool PlayOnAwake = false;
        
        [SerializeField]
        protected SequenceAsset _sequenceAsset;
        public SequenceAsset SequenceAsset => _sequenceAsset;

        [ShowInInspector,ShowIf("@_runtimeTempSequence!=null")]
        private ISequence _runtimeTempSequence;
        public ISequence Sequence => _sequenceAsset ?? _runtimeTempSequence;

        [NonSerialized] protected SequenceRunner _runner;
        public SequenceRunner Runner => _runner;
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
            if (_runner != null)
            {
                if (_runner.runnerState == ERunnerState.None)
                {
                    _runner.OnStart();
                    _state = EState.Running;
                }
                else if(_runner.runnerState == ERunnerState.Diposed)
                {
                    _runner.Release();
                    _runner = null;
                    _state = EState.Done;
                }
                else if(_runner.runnerState == ERunnerState.Paused)
                {
                    _state = EState.Paused;
                }
                else if(_runner.runnerState == ERunnerState.Failure)
                {
                    _state = EState.Failure;
                }
                else if(_runner.runnerState == ERunnerState.Done)
                {
                    _state = EState.Done;
                }
                else //_sequenceRunner.State == SequenceRunner.EState.Running
                {
                    _runner.OnUpdate(deltaTime, force);
                }
            }
        }

        [NonSerialized] private SequenceHandle _sequenceHandle;
        [NonSerialized] private PreviewSequenceHandle _previewSequenceHandle;
        public ISequenceHandle GetHandle()
        {
            if (!EditorApplication.isPlaying)
                return GetPreviewHandle();
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
        /// <returns></returns>
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
        /// <returns></returns>
        private ISequenceHandle GetRuntimeHandle()
        {
            _previewSequenceHandle?.Release();
            _previewSequenceHandle = null;
            if (_sequenceHandle == null)
                _sequenceHandle = SequenceHandle.Get(this);
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
            if (_runner == null || !_runner.IsRunning)
                return;
            _runner.Pause();
            _state = EState.Paused;
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