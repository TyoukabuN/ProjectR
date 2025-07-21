using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using PJR.Timeline.Pool;
using UnityEngine.Pool;

namespace PJR.Timeline
{
    public class UsingTempSequence : SerializedMonoBehaviour
    {
        public SequenceDirector SequenceDirector;

        [Button]
        void RunTest()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityEngine.Debug.Log("Only working in playmode.");
                return;
            }
            SequenceDirector ??= gameObject.GetComponent<SequenceDirector>();
            if (SequenceDirector == null)
                return;
            SequenceDirector.SetRuntimeTempSequence(GetTempSequence());
            SequenceDirector.Play();
        }

        private ISequence _runtimeTemplateSequence;

        public AnimationClip AnimationClip; 
        ISequence GetTempSequence()
        {
            if (_runtimeTemplateSequence == null)
            {
                var temp = RuntimeTemplateSequence.Get();
                var clip = new AnimancerClip();
                temp.AddClip(AnimancerClip.GetRuntimeTemplate(AnimationClip,60));
                _runtimeTemplateSequence = temp;
            }

            return _runtimeTemplateSequence;
        }
    }

    public partial class RuntimeTemplateSequence : PoolableObject, ISequence , ITrack
    {
        public bool RuntimeTempInstance => true;
        public Define.EFrameRate FrameRateType { get; set; }

        public List<Track> Tracks
        {
            get => null;
            set { }
        }

        private List<Clip> _clips;
        public List<Clip> Clips => _clips;
        
        public bool Valid => (_clips?.Count ?? 0) > 0;

        public SequenceRunner GetRunner(GameObject gameObject)
        {
            var temp = Runner.Get();
            temp.Reset(gameObject, this);
            return temp;
        }

        public void AddClip(Clip clip)
        {
            if (clip == null)
                return;
            _clips ??= ListPool<Clip>.Get();
        }

        public override void Clear()
        {
            if (_clips != null)
            {
                ListPool<Clip>.Release(_clips);
                _clips = null;
            }
        }

        #region Pool

        public static RuntimeTemplateSequence Get() => Pool.ObjectPool<RuntimeTemplateSequence>.Get();
        public override void Release() => Pool.ObjectPool<RuntimeTemplateSequence>.Release(this);

        #endregion
    }

    public partial class RuntimeTemplateSequence
    {
        public class Runner : SequenceRunner
        {
            private TrackRunner _trackRunner; 
            public override void Clear()
            {
            }

            public override void OnStart()
            {
                if (State >= EState.Done)
                    return;
                Debug.Log("RuntimeTemplateSequence.Runner.OnStart");
                State = EState.Running;

            }

            public override void OnUpdate(float deltaTime, bool force = false)
            {
            }

            public void Reset(GameObject gameObject, RuntimeTemplateSequence templateSequence)
            {
                var trackRunner = TrackRunner.Get();
                trackRunner.Reset(templateSequence, templateSequence);
                if (trackRunner.Invalid)
                {
                    trackRunner.Release();
                    return;
                }
                
                _trackRunner = trackRunner;
            }
            #region Pool

            public static Runner Get() => Pool.ObjectPool<Runner>.Get();
            public override void Release() => Pool.ObjectPool<Runner>.Release(this);

            #endregion
        }
    }
}