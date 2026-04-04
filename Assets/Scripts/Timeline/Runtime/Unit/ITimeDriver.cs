using System;
using UnityEngine;

namespace PJR.Timeline
{
    /// <summary>
    /// 时间驱动策略
    /// 怎样调用SequenceRunner之类的Runner提供的Update方法
    /// 负责产出 UpdateContext 并通过回调传出
    /// </summary>
    public interface ITimeDriver
    {
        void Drive(float deltaTime, TimeDriverContext shared, Action<UpdateContext> onUpdate);
        void Reset();
    }

    /// <summary>
    /// Driver 所需的共享上下文（由 SequenceRunner 提供）
    /// </summary>
    public struct TimeDriverContext
    {
        public double timeScale;
        public double currentTime;
        public int currentFrame;
        public GameObject gameObject;
    }
}
