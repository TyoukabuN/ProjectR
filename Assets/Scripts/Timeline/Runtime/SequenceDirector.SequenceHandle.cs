using System;
using PJR.Timeline.Pool;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        public class SequenceHandle : PoolableObject, ISequencePlayableHandle, IDisposable
        {
            public double time
            {
                get
                {
                    if (!Valid || _director._sequenceRunner == null)
                        return 0;
                    return _director._sequenceRunner.TotalTime;
                }
                set
                {
                    if (!Valid || _director._sequenceRunner == null)
                        return;
                    _director._sequenceRunner.TotalTime = value;   
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
                var temp = ObjectPool<SequenceHandle>.Get();
                temp._director = director;
                return temp;
            }
            public void Dispose()
            {
                _director = null;
            }
            public override void Release() => ObjectPool<SequenceHandle>.Release(this);
            public double ToGlobalTime(double t) => t;
            public double ToLocalTime(double t) => t;

            bool ISequencePlayableHandle.IsPlaying()
            {
                if (!Valid || _director._sequenceRunner == null)
                    return false;
                return _director._sequenceRunner.IsRunning;
            }
            void ISequencePlayableHandle.Play()
            {
                if (!Valid) 
                    return;
                if (_director._sequenceRunner != null)
                {
                    if (_director._sequenceRunner.State == SequenceRunner.EState.Diposed)
                    {
                        _director._sequenceRunner.Release();
                        _director._sequenceRunner = null;
                    }
                    else
                        _director._sequenceRunner.State = SequenceRunner.EState.Running;
                }

                
                
                if (_director._sequenceRunner == null)
                    _director._sequenceRunner = _director.GetRunner();
            }
            void ISequencePlayableHandle.Pause()
            {
                if (!Valid || _director._sequenceRunner == null) 
                    return;
                _director._sequenceRunner.State = SequenceRunner.EState.Paused;
            }
            void ISequencePlayableHandle.Reset()
            {
                if (!Valid || _director._sequenceRunner == null) 
                    return;
                _director._sequenceRunner.Release();
            }
        }
    }
}