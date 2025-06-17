using UnityEngine;

namespace PJR.Timeline
{
    public class MonoSequenceHandle : MonoBehaviour, ISequenceHandle
    {
        [SerializeField]
        private Sequence sequence;
        public Sequence Sequence => sequence;
    }
}