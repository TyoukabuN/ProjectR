using System.Collections.Generic;
using PJR.Timeline.Pool;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Timeline
{
    /// <summary>
    /// runtime拼凑的sequence
    /// </summary>
    public partial class RuntimeTemplateSequence : PoolableObject, ISequence , ITrack
    {
        public bool RuntimeTempInstance => true;
        public Define.EFrameRate FrameRateType { get; set; }

        // ISequence.Tracks 显式实现
        IReadOnlyList<ITrack> ISequence.Tracks => null;

        private List<Clip> _clips;
        // ITrack.Clips 显式实现
        IReadOnlyList<IClip> ITrack.Clips => _clips;
        
        public bool Valid => (_clips?.Count ?? 0) > 0;

        public int Frames
        {
            get
            {
                if (_clips == null)
                    return 0;
                int maxEndFrame = 0;
                for (int i = 0; i < _clips.Count; i++)
                {
                    var clip = _clips[i];
                    if (clip == null)
                        continue;
                    if (clip.EndFrame > maxEndFrame)
                        maxEndFrame = clip.EndFrame;
                }
                return maxEndFrame;
            }
        }

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