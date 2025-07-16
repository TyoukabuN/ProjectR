using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    [ExecuteInEditMode]
    public partial class SequenceDirector : SerializedMonoBehaviour, ISequenceHolder
    {
        public bool PlayOnAwake = false;
        
        [SerializeField]
        protected SequenceAsset _sequenceAsset;
        public SequenceAsset SequenceAsset => _sequenceAsset;

        [NonSerialized] protected SequenceRunner _sequenceRunner;
        public void Start()
        {
            if (PlayOnAwake)
                GetRunner();
        }
        private void Update()
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
                else
                {
                    UnityEngine.Debug.Log($"[SequenceDirector.Update] {Time.unscaledDeltaTime}]");
                    _sequenceRunner.OnUpdate(Time.unscaledDeltaTime);
                }
            }
        }
        [NonSerialized] private SequenceHandle _sequenceHandle;
        public ISequenceHandle GetHandle()
        {
            if (_sequenceHandle == null)
                _sequenceHandle = SequenceHandle.Get(this);
            return _sequenceHandle;
        }
      
        public SequenceRunner GetRunner()
        {
            if (_sequenceRunner == null)
            {
                _sequenceRunner = SequenceRunner.Get();
                _sequenceRunner.Reset(gameObject, SequenceAsset);
            }
            return _sequenceRunner;
        }
    }
}