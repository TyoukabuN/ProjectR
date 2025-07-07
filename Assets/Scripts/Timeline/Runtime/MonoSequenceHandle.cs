using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Timeline
{
    public class MonoSequenceHandle : SerializedMonoBehaviour, ISequenceHandle
    {
        [SerializeField]
        private Sequence _sequence;
        public Sequence Sequence => _sequence;

        [NonSerialized] private SequenceRunner _sequenceRunner;
        public SequenceRunner GetSequenceRunner()
        {
            if (_sequenceRunner != null)
            {
                _sequenceRunner.Release();
                _sequenceRunner = null;
            }
            _sequenceRunner = SequenceRunner.Get();
            _sequenceRunner.Reset(gameObject, Sequence);
            return _sequenceRunner;
        }
    }
}