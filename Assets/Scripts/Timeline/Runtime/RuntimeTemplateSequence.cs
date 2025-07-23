using System.Collections.Generic;
using PJR.Timeline.Pool;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Timeline
{
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
            _clips.Add(clip);
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
}