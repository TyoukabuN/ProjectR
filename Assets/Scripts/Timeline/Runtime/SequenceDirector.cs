using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Timeline
{
    public class SequenceDirector : SerializedMonoBehaviour, ISequenceHolder
    {
        public bool PlayOnAwake = false;
        
        [SerializeField]
        protected SequenceAsset _sequenceAsset;
        public SequenceAsset SequenceAsset => _sequenceAsset;

        [NonSerialized] protected SequenceRunner _sequenceRunner;
        public SequenceRunner SequenceRunner => _sequenceRunner ?? GetRunner();

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
                else
                {
                    _sequenceRunner.OnUpdate(Time.unscaledDeltaTime);
                }
            }
        }
        public void Play()
        {
            GetRunner();
        }
        [NonSerialized] private ISequenceHandle _sequenceHandle;
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
        
        public class SequenceHandle : ISequenceHandle
        {
            public double time
            {
                get
                {
                    if (!Valid || _director.SequenceRunner == null)
                        return 0;
                    return _director.SequenceRunner.TotalTime;
                }
                set
                {
                    if (!Valid || _director.SequenceRunner == null)
                        return;
                    _director.SequenceRunner.TotalTime = value;   
                }
            }
            public bool Valid => Director != null;
            public SequenceAsset SequenceAsset => Director?.SequenceAsset;
            public SequenceDirector Director => _director;
            private SequenceDirector _director;
            
            public SequenceHandle(){}
            public SequenceHandle(SequenceDirector director)=>_director = director;
            public static SequenceHandle Get(SequenceDirector director)
            {
                var temp = UnityEngine.Pool.GenericPool<SequenceHandle>.Get();
                temp._director = director;
                return temp;
            }
            public void Release() => UnityEngine.Pool.GenericPool<SequenceHandle>.Release(this);
            public double ToGlobalTime(double t) => t;
            public double ToLocalTime(double t) => t;
        }
    }
}