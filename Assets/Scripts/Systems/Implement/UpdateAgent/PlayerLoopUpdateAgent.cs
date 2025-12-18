using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace PJR.Systems
{
    public class PlayerLoopUpdateAgent : MonoSingleton<PlayerLoopUpdateAgent>
    {
        /// <summary>
        /// Unity里的PlayerLoop大分类
        /// </summary>
        public enum EPlayerLoopStage
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

        private static List<AgentLocation> CarePlayerLoopSystem = new()
        {
            new(typeof(Initialization), null, AgentLocation.EOrder.After),
            
            new(typeof(EarlyUpdate), typeof(EarlyUpdate.ScriptRunDelayedStartupFrame), AgentLocation.EOrder.Before),
            
            new(typeof(FixedUpdate), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), AgentLocation.EOrder.Before),
            
            new(typeof(PreUpdate), typeof(PreUpdate.PhysicsUpdate), AgentLocation.EOrder.Before),
            
            new(typeof(Update), typeof(Update.ScriptRunBehaviourUpdate), AgentLocation.EOrder.Before),
            
            new(typeof(PreLateUpdate), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), AgentLocation.EOrder.Before),
            new(typeof(PreLateUpdate), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), AgentLocation.EOrder.After),
            
            new(typeof(PostLateUpdate), typeof(PostLateUpdate.PlayerSendFrameComplete), AgentLocation.EOrder.After),
        };
        
        public struct AgentLocation
        {
            public enum EOrder
            {
                Before,
                After
            }
            
            public Type PlayerLoopSystemType;
            public Type SubPlayerLoopSystemType;
            public EOrder Order;
            public AgentLocation(Type playerLoopSystemType, Type subPlayerLoopSystemType, EOrder order)
            {
                PlayerLoopSystemType = playerLoopSystemType;
                SubPlayerLoopSystemType = subPlayerLoopSystemType;
                Order = order;
            }
        }


        public interface IUpdatable
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
        
        private static List<WeakReference<IUpdatable>> _updatables;
        public static List<WeakReference<IUpdatable>> Updatables => _updatables ??= new(128);

        public static bool Register(IUpdatable updatable)
        {
            if (updatable == null)
                return false;
            if (!IsContains(updatable))
                return false;
            Updatables.Add(new WeakReference<IUpdatable>(updatable));
            return true;
        }
        public static bool UnRegister(IUpdatable updatable)
        {
            if (updatable == null)
                return false;
            if (IsContains(updatable))
                return false;
            return true;
        }


        private static bool IsContains(IUpdatable updatable) => IsContains(updatable, out _);
        private static bool IsContains(IUpdatable updatable, out int index)
        {
            index = -1;
            if (updatable == null)
                return false;
            for (var i = 0; i < Updatables.Count; i++)
            {
                var item = Updatables[i];
                if (item == null)
                    continue;
                if (!item.TryGetTarget(out var target))
                    continue;
                index = i;
                if (updatable == target)
                    return true;
            }

            return false;
        }

        private void Update()
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
    }
}