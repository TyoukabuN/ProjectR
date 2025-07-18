using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class SequenceDirector : SerializedMonoBehaviour, ISequenceHolder
    {
        public bool PlayOnAwake = false;
        
        [SerializeField]
        protected SequenceAsset _sequenceAsset;
        public SequenceAsset SequenceAsset => _sequenceAsset;

        [NonSerialized] protected SequenceRunner _sequenceRunner;
        public SequenceRunner SequenceRunner => _sequenceRunner;
        public void Start()
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
            if (_sequenceRunner != null)
            {
                if (_sequenceRunner.State == SequenceRunner.EState.None)
                {
                    _sequenceRunner.OnStart();
                }
                else if(_sequenceRunner.State == SequenceRunner.EState.Diposed)
                {
                    _sequenceRunner.Release();
                    _sequenceRunner = null;
                }
                else if(_sequenceRunner.State == SequenceRunner.EState.Paused)
                {
                }
                else
                {
                    _sequenceRunner.OnUpdate(deltaTime, force);
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

        private ISequenceHandle GetPreviewHandle()
        {
            _sequenceHandle?.Release();
            _sequenceHandle = null;
            if (_previewSequenceHandle == null)
                _previewSequenceHandle = PreviewSequenceHandle.Get(this);
            return _previewSequenceHandle;
        }
        
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
            if (_sequenceRunner == null || _sequenceRunner.IsDisposed)
            {
                _sequenceRunner = SequenceRunner.Get();
                _sequenceRunner.Reset(gameObject, SequenceAsset);
            }
            return _sequenceRunner;
        }

        public void Stop()
        {
            _sequenceRunner?.Release();
            _sequenceRunner = null;
        }
    }
}