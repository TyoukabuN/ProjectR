using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Pool;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    public class TrackRunner : UnitRunner<ClipRunner>, IErrorRecorder
    {
        public List<ClipRunner> clipRunners => _subRunners;
        private ITrack _track;
        Clip2ClipHandleFunc _clip2ClipHandle;

        public bool Invalid => runnerState >= ERunnerState.Diposed || AnyError;

        public TrackRunner()
        {
            Clear();
            runnerState = ERunnerState.None;
        }
        protected override void OnClear()
        {
            if (_subRunners != null)
            {
                foreach (var runner in _subRunners)
                    runner.Release(); // 归还 ClipRunner 到对象池
                CollectionPool<List<ClipRunner>, ClipRunner>.Release(_subRunners);
                _subRunners = null;
            }
            _track = null;
            _clip2ClipHandle = null;
        }

        public virtual bool Reset(ISequence sequence, ITrack track) => Reset(sequence, track, Global.Clip2ClipHandleFunc);
        public virtual bool Reset(ISequence sequence, ITrack track, Clip2ClipHandleFunc clip2ClipHandle)
        {
            Sequence = sequence;
            _track = track;
            _clip2ClipHandle = clip2ClipHandle;

            if (_track == null)
            {
                AsFailure(ErrCode_TrackRuner_TrackIsNull);
                return false;
            }
            if (_clip2ClipHandle == null)
            {
                AsFailure(ErrCode_TrackRuner_Clip2ClipHandle);
                return false;
            }
            if (_track.Clips == null)
            {
                AsFailure(ErrCode_TrackRuner_ClipsIsNull);
                return false;
            }
            if (_track.Clips.Count <= 0)
            {
                AsFailure(ErrCode_TrackRuner_NoneClip);
                return false;
            }

            _subRunners = CollectionPool<List<ClipRunner>, ClipRunner>.Get();
            for (int i = 0; i < _track.Clips.Count; i++)
            {
                var clip = _track.Clips[i];
                if (clip == null)
                    continue;
                ClipRunner clipRunner = null;

#if UNITY_EDITOR
                if(!EditorApplication.isPlaying)
                    clipRunner =clip.Editor_GetPreviewRunner();
                else
#endif
                    clipRunner =clip.GetRunner();
                
                if (clipRunner == null)
                    continue;
                _subRunners.Add(clipRunner);
            }

            ForeachSubRunner(InitClipRunner);

            runnerState = ERunnerState.None;
            return true;
        }
        public virtual void OnInit()
        {
            runnerState = ERunnerState.None;
        }
        public virtual void OnStart()
        {
            runnerState = ERunnerState.Running;
        }

        public override void OnUpdate(UpdateContext context)
        {
            if (runnerState >= ERunnerState.Done)
                return;
            if (_subRunners == null)
            {
                runnerState = ERunnerState.Done;
                return;
            }

            bool allDone = true;
            for (int i = 0; i < _subRunners.Count; i++)
            {
                var clipRunner = _subRunners[i];
                if (!IsClipRunnerUpdatable(clipRunner))
                    continue;

                clipRunner.SetUpdateContext(context);

                bool isOutOfRange = clipRunner.Clip.OutOfRange(context.currentTime, Sequence.FrameRateType.SPF());
                if (isOutOfRange)
                {
                    if (clipRunner.Running)
                        clipRunner.End();
                    else if (clipRunner.WaitingForStart)
                    {
                        // future clip（还没到开始时间）：保持 allDone=false，不能让轨道提前结束
                        // past clip（已过结束时间但从未被执行，如 SeekTo 跳过）：视为已完成，不影响 allDone
                        if (context.currentTime < clipRunner.Clip.start)
                            allDone = false;
                    }
                    else if (clipRunner.IsPaused)
                        allDone = false;
                    continue;
                }

                if (clipRunner.WaitingForStart)
                    clipRunner.OnStart(context);
                if (clipRunner.Running)
                    clipRunner.OnUpdate(context);

                if (clipRunner.runnerState < ERunnerState.Done)
                    allDone = false;
            }

            runnerState = allDone ? ERunnerState.Done : runnerState;
        }
        
        protected override void Internal_OnDone()
        {
        }

        protected override void OnPlay()
        {
            runnerState = ERunnerState.Running;
            // 重置 Done 和 Paused 状态的 ClipRunner 到 None，让 OnUpdate 按时间范围自然驱动生命周期
            // 不能直接 Play() 所有 ClipRunner，否则未到时间的 Clip 会被 OutOfRange 判定立即 End
            ForeachSubRunner(sub =>
            {
                if (sub.IsDone || sub.IsPaused)
                    sub.runnerState = ERunnerState.None;
            });
        }

        protected override void OnPause()
        {
            runnerState = ERunnerState.Paused;
            ForeachSubRunner(sub => sub.Pause());
        }

        void InitClipRunner(ClipRunner clipHandle) => clipHandle?.OnInit();

        /// <summary>
        /// 为 SeekTo 做准备：重置所有 ClipRunner 为 None，让 OnUpdate 在指定时间点重新评估
        /// </summary>
        public void PrepareForSeek()
        {
            runnerState = ERunnerState.Running;
            ForeachSubRunner(sub => sub.runnerState = ERunnerState.None);
        }

        public bool IsClipRunnerUpdatable(ClipRunner clipHandle)
        {
            if (clipHandle == null)
                return false;
            if (clipHandle.runnerState >= ERunnerState.Done)
                return false;
            return true;
        }
        double GetSecondPerFrame() => Utility.GetSecondPerFrame(Sequence?.FrameRateType ?? EFrameRate.Game);

        #region IErrorRecorder Impl
        string IErrorRecorder.Error => base.Error;
        bool IErrorRecorder.AnyError => base.AnyError;
        #endregion

        #region Pool
        public static TrackRunner Get() => Pool.ObjectPool<TrackRunner>.Get();
        public override void Release() => Pool.ObjectPool<TrackRunner>.Release(this);
        #endregion
    }
}
