using PJR.Core.PlayerLoopAgent;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerloopUpdateTest : MonoBehaviour
{
    public enum EAgentUpdatePhase
    {
        Initialization,
        EarlyUpdate,
        FixedUpdate,
        PreUpdate,
        Update,
        PreLateUpdate,
        PostScriptLateUpdate,
        PostLateUpdate,
    }

    [Button]
    public void DoTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        
        var tree = PlayerLoopTree.GetTreeByCurrentPlayerLoop();

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

    [Button]
    public void Log()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        string str = "";
        LogPlayerLoopSystem(PlayerLoop.GetCurrentPlayerLoop(), ref str);
        Debug.Log(str);
    }

    public static void Internal_OnPlayerLoopPhase(EAgentUpdatePhase phase)
    {
        Debug.Log( phase);
    }

    private string GetPlayerLoopSystemName(PlayerLoopSystem loopSys)
    {
        if (loopSys.type != null)
        {
            if (!string.IsNullOrEmpty(loopSys.type.Namespace) && loopSys.type.Namespace.StartsWith("UnityEngine"))
            {
                return loopSys.ToString();
            }
            else
            {
                return $"[Non-native] {loopSys.ToString()}";
            }
        }
        return "Unknown";
    }
    private string LogPlayerLoopSystem(PlayerLoopSystem loopSys, ref string str, int depth = 0)
    {
        string _str = "";
        if (loopSys.type != null)
        {
            if (loopSys.subSystemList == null)
            {
                _str = $"   L {GetPlayerLoopSystemName(loopSys)}";
            }
            else
            {
                _str = GetPlayerLoopSystemName(loopSys);
            }
        }

        if (string.IsNullOrEmpty(str))
            str = _str;
        else
            str += ("\n" + _str);

        if (loopSys.subSystemList != null)
        {
            foreach (var sub in loopSys.subSystemList)
            {
                LogPlayerLoopSystem(sub, ref str, ++depth);
            }
        }

        return str;
    }

    public static class DefaultAgents
    {
        public struct Initialization : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.Initialization), 
                null,
                AgentLocation.EOrder.After);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(Initialization),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.Initialization),
            };
        }

        public struct EarlyUpdate : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.EarlyUpdate), 
                typeof(UnityEngine.PlayerLoop.EarlyUpdate.ScriptRunDelayedStartupFrame), 
                AgentLocation.EOrder.Before);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(EarlyUpdate),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.EarlyUpdate),
            };
        }

        public struct FixedUpdate : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.FixedUpdate), 
                typeof(UnityEngine.PlayerLoop.FixedUpdate.ScriptRunBehaviourFixedUpdate), 
                AgentLocation.EOrder.Before);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(FixedUpdate),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.FixedUpdate),
            };
        }

        public struct PreUpdate : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.PreUpdate), 
                typeof(UnityEngine.PlayerLoop.PreUpdate.PhysicsUpdate), 
                AgentLocation.EOrder.Before);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(FixedUpdate),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.FixedUpdate),
            };
        }

        public struct Update : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.Update), 
                typeof(UnityEngine.PlayerLoop.Update.ScriptRunBehaviourUpdate), 
                AgentLocation.EOrder.Before);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(Update),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.Update),
            };
        }

        public struct PreLateUpdate : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.PreLateUpdate), 
                typeof(UnityEngine.PlayerLoop.PreLateUpdate.ScriptRunBehaviourLateUpdate), 
                AgentLocation.EOrder.Before);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(PreLateUpdate),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PreLateUpdate),
            };
        }

        public struct PostScriptLateUpdate : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.PreLateUpdate), 
                typeof(UnityEngine.PlayerLoop.PreLateUpdate.ScriptRunBehaviourLateUpdate),
                AgentLocation.EOrder.After);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(PostScriptLateUpdate),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PostScriptLateUpdate),
            };
        }

        public struct PostLateUpdate : ILocatedAgent
        {
            public AgentLocation AgentLocation => new(
                typeof(UnityEngine.PlayerLoop.PostLateUpdate), 
                typeof(UnityEngine.PlayerLoop.PostLateUpdate.PlayerSendFrameComplete),
                AgentLocation.EOrder.After);

            public PlayerLoopSystem GetPlayerLoopSystem() => new()
            {
                type = typeof(PostLateUpdate),
                updateDelegate = () => Internal_OnPlayerLoopPhase(EAgentUpdatePhase.PostLateUpdate),
            };
        }

    }
}
