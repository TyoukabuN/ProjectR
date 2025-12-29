using System;
using UnityEngine.LowLevel;

namespace PJR.Core.PlayerLoopAgent
{
    /// <summary>
    /// 能够提供PlayerLoopSystem的对象
    /// </summary>
    public interface IAgent
    {
        public PlayerLoopSystem GetPlayerLoopSystem();
    }

    /// <summary>
    /// 有AgentLocation定位信息的IAgent
    /// </summary>
    public interface ILocatedAgent : IAgent
    {
        public AgentLocation AgentLocation { get; }
    }

    /// <summary>
    /// 用来定位在<see cref="PlayerLoopSystem"/>中的位置
    /// </summary>
    public struct AgentLocation
    {
        public static AgentLocation Empty => new();

        public bool IsEmpty => !_notEmpty;

        public enum EOrder
        {
            Before,
            After
        }

        /// <summary>
        /// 要插到哪个类型的下
        /// </summary>
        public Type TargetType;

        /// <summary>
        /// 用于定位的TargetType下Sub里类型，
        /// 如果为空，会根据
        /// </summary>
        public Type AnchorType;

        public EOrder Order;

        private bool _notEmpty;

        public AgentLocation(Type targetType, Type anchorType, EOrder order)
        {
            TargetType = targetType;
            AnchorType = anchorType;
            Order = order;
            _notEmpty = true;
        }
    }
}