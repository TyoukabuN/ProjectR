using PJR.Core.PlayerLoopAgent;
using UnityEngine.LowLevel;

namespace PJR.Systems.PlayerLoopUpdateAgent
{
    public partial class PlayerLoopUpdateAgentSystem
    {
        /// <summary>
        /// PlayerLoopUpdateAgent内部的定义好点Agent，
        /// UpdateFunction统一是Internal_OnPlayerLoopPhase，
        /// </summary>
        public static class DefaultAgents
        {
            public struct Initialization : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.Initialization), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(Initialization),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.Initialization),
                };
            }

            public struct EarlyUpdate : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.EarlyUpdate), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(EarlyUpdate),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.EarlyUpdate),
                };
            }

            public struct FixedUpdate : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.FixedUpdate), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(FixedUpdate),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.FixedUpdate),
                };
            }

            public struct PreUpdate : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.PreUpdate), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem.UpdateFunction GetUpdateFunction() =>
                    () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PreUpdate);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(FixedUpdate),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.FixedUpdate),
                };
            }

            public struct Update : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.Update), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(Update),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.Update),
                };
            }

            public struct PreLateUpdate : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.PreLateUpdate), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(PreLateUpdate),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PreLateUpdate),
                };
            }

            public struct PostScriptLateUpdate : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.PostLateUpdate), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(PostScriptLateUpdate),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PostScriptLateUpdate),
                };
            }

            public struct PostLateUpdate : ILocatedAgent
            {
                public AgentLocation AgentLocation => new(typeof(UnityEngine.PlayerLoop.PostLateUpdate), null, AgentLocation.EOrder.After);
                public PlayerLoopSystem GetPlayerLoopSystem() =>new()
                {
                    type = typeof(PostLateUpdate),
                    updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PostLateUpdate),
                };
            }

        }
    }
}