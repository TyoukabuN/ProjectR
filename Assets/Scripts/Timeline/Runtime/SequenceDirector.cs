using UnityEngine;
using UnityEngine.Serialization;

namespace PJR.Timeline
{
    public class SequenceDirector : MonoSequenceHandle
    {
    }

    public class MonoSequenceHandle : MonoBehaviour, ISequenceHandle
    {
        [SerializeField]
        private Sequence sequence;
        public Sequence Sequence => sequence;
    }
}