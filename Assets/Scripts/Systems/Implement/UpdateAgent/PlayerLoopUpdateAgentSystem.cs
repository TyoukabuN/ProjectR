using System;
using System.Collections.Generic;
using Sirenix.Reflection.Editor;
using UnityEngine;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;

namespace PJR.Systems.PlayerLoopUpdateAgent
{
    public partial class PlayerLoopUpdateAgentSystem : MonoSingleton<PlayerLoopUpdateAgentSystem>
    {
        /// <summary>
        /// Unity里的PlayerLoop大分类
        /// </summary>
        public enum EAgentUpdatePhase
        {
            None = 0,
            Initialization = 1,
            EarlyUpdate = 2,
            FixedUpdate = 3,
            PreUpdate = 4,
            Update = 5,
            PreLateUpdate = 6,
            PostScriptLateUpdate = 8,
            PostLateUpdate = 7
        }

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
        
        protected override void OnInstantiated()
        {
            var tree = PlayerLoopTree.GetTreeByDefaultPlayerLoop();
            
            tree.AddNode(new DefaultAgents.Initialization());
            tree.AddNode(new DefaultAgents.EarlyUpdate());
            tree.AddNode(new DefaultAgents.FixedUpdate());
            tree.AddNode(new DefaultAgents.PreUpdate());
            tree.AddNode(new DefaultAgents.Update());
            tree.AddNode(new DefaultAgents.PreLateUpdate());
            tree.AddNode(new DefaultAgents.PostScriptLateUpdate());
            tree.AddNode(new DefaultAgents.PostLateUpdate());
            
            tree.SetPlayerLoop();
        }
        
        public struct Token
        {
            public EAgentUpdatePhase Phase;
            public int Id;
            public Token(EAgentUpdatePhase phase, int id)
            {
                Phase = phase;
                Id = id;
            }
        }
        public delegate void PhaseAction();

        private Dictionary<EAgentUpdatePhase, Dictionary<int, PhaseAction>> Phase2actionsMap
            => _phase2actionsMap ??= new();

        private Dictionary<EAgentUpdatePhase, Dictionary<int,PhaseAction>> _phase2actionsMap;
        private static int _guid = 0;

        protected override void OnUpdate(float deltaTime)
        {
            if (Updatables == null)
                return;
            for (var i = 0; i < Updatables.Count; i++)
            {
                var item = Updatables[i];
                if (item == null)
                    continue;
                if(!item.TryGetTarget(out var updatable))
                    continue;
                updatable.OnUpdate(Time.deltaTime);
            }
        }
        
        public Token AddPhaseAction(EAgentUpdatePhase phase, PhaseAction action)
        {
            GetPhaseActionMap(phase, out var actionMap);
            int id = ++_guid;
            actionMap.Add(id,action);
            return new(phase, id);
        }
        
        public bool RemovePhaseAction(Token token)
        {
            if (!Phase2actionsMap.TryGetValue(token.Phase, out var actions))
                return false;

            return actions.Remove(token.Id, out var action);
        }

        private bool GetPhaseActionMap(EAgentUpdatePhase phase , out Dictionary<int, PhaseAction> actionMap)
        {
            if (!Phase2actionsMap.TryGetValue(phase, out actionMap))
            {
                actionMap = new Dictionary<int,PhaseAction>(64);
                Phase2actionsMap[phase] = actionMap;
            }
            return true;
        }

        private static void Internal_OnPlayerLoopPhase(EAgentUpdatePhase phase)
        {
            if (Instance?.GetPhaseActionMap(phase, out var actionMap) ?? false)
            {
                foreach (var pair in actionMap)
                {
                    var id = pair.Key;
                    var action = pair.Value;
                    if(action == null)
                        continue;
                    try
                    {
                        action.Invoke();
                    }
                    catch(Exception e)
                    {
                        LogSystem.LogWarning(e);
                    }
                }
            }
            if (phase == EAgentUpdatePhase.Initialization)
            {
                
            }
            if (phase == EAgentUpdatePhase.EarlyUpdate)
            {
                
            }
            if (phase == EAgentUpdatePhase.FixedUpdate)
            {
                
            }
            if (phase == EAgentUpdatePhase.PreUpdate)
            {
                
            }
            if (phase == EAgentUpdatePhase.Update)
            {
                
            }
            if (phase == EAgentUpdatePhase.PreLateUpdate)
            {
                
            }
            if (phase == EAgentUpdatePhase.PostScriptLateUpdate)
            {
                
            }
            if (phase == EAgentUpdatePhase.PostLateUpdate)
            {
                
            }
        }

        public interface IUpdateTerm
        {
            public int UpdateFrameInterval => 0;
            public void OnUpdate(UpdateContext context);
        }
        public struct UpdateContext
        {
            public float Delta;

            public static implicit operator UpdateContext(float delta)
                => new() { Delta = delta };
        }

        private static List<WeakReference<IUpdateTerm>> _updatables;
        public static List<WeakReference<IUpdateTerm>> Updatables => _updatables ??= new(128);

        public static bool Register(IUpdateTerm updateTerm)
        {
            if (updateTerm == null)
                return false;
            if (!IsContains(updateTerm))
                return false;
            Updatables.Add(new WeakReference<IUpdateTerm>(updateTerm));
            return true;
        }
        public static bool UnRegister(IUpdateTerm updateTerm)
        {
            if (updateTerm == null)
                return false;
            if (IsContains(updateTerm))
                return false;
            return true;
        }


        private static bool IsContains(IUpdateTerm updateTerm) => IsContains(updateTerm, out _);
        private static bool IsContains(IUpdateTerm updateTerm, out int index)
        {
            index = -1;
            if (updateTerm == null)
                return false;
            for (var i = 0; i < Updatables.Count; i++)
            {
                var item = Updatables[i];
                if (item == null)
                    continue;
                if (!item.TryGetTarget(out var target))
                    continue;
                index = i;
                if (updateTerm == target)
                    return true;
            }

            return false;
        }


    }
}