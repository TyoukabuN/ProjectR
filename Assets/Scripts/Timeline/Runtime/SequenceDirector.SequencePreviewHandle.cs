using PJR.Timeline.Pool;

namespace PJR.Timeline
{
    public partial class SequenceDirector
    {
        public class PreviewSequenceHandle : SequenceHandle
        {
            public override float time
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
                    SequenceRunner.TotalTime = value;   
                }
            }
            public static PreviewSequenceHandle Get(SequenceDirector director)
            {
                var temp = ObjectPool<PreviewSequenceHandle>.Get();
                temp._director = director;
                return temp;
            }
            public override void Clear()
            {
                _director = null;
            }
            public override void Release() => ObjectPool<PreviewSequenceHandle>.Release(this);
            public double ToGlobalTime(double t) => t;
            public double ToLocalTime(double t) => t;

            public override bool IsPlaying()
            {
                if (!Valid || _director._sequenceRunner == null)
                    return false;
                return _director._sequenceRunner.IsRunning;
            }
            public override void Play()
            {
                if (!Valid) 
                    return;
                if (_director._sequenceRunner == null)
                    _director._sequenceRunner = _director.GetRunner();
                
                if (_director._sequenceRunner != null)
                {
                    if (_director._sequenceRunner.State == SequenceRunner.EState.None)
                    {
                        _director._sequenceRunner.OnStart();
                    }
                    else if (_director._sequenceRunner.State == SequenceRunner.EState.Paused)
                    {
                        _director._sequenceRunner.State = SequenceRunner.EState.Running;
                    }
                    else if (_director._sequenceRunner.State == SequenceRunner.EState.Diposed)
                    {
                        _director._sequenceRunner.Release();
                        _director._sequenceRunner = null;
                    }
                }
            }
            public override void Pause()
            {
                if (!Valid || _director._sequenceRunner == null) 
                    return;
                _director._sequenceRunner.State = SequenceRunner.EState.Paused;
            }
            public override void Stop()
            {
                if (!Valid) 
                    return;
                _director.Stop();
            }
        }
    }
}