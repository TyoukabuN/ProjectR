using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public class UsingTempSequence : SerializedMonoBehaviour
    {
        public SequenceDirector SequenceDirector;

        [Button]
        void RunTest()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = true;
                return;
            }

            SequenceDirector ??= gameObject.GetComponent<SequenceDirector>();
            if (SequenceDirector == null)
                return;
            SequenceDirector.SetRuntimeTempSequence(GetTempSequence());
            SequenceDirector.Replay();
        }

        private ISequence _runtimeTemplateSequence;

        public AnimationClip AnimationClip; 
        ISequence GetTempSequence()
        {
            if (_runtimeTemplateSequence == null)
            {
                var temp = RuntimeTemplateSequence.Get();
                temp.AddClip(AnimancerClip.GetRuntimeTemplate(AnimationClip,50));
                _runtimeTemplateSequence = temp;
            }
            return _runtimeTemplateSequence;
        }
    }
}