using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace PJR.Core.PlayerLoopAgent
{
    public partial class PlayerLoopTree
    {
        public class Node
        {
            public PlayerLoopTree Tree { get; private set; }
            public Node Parent { get; private set; }
            public List<Node> SubNodes = new(64);
            public PlayerLoopSystem PlayerLoopSystem;

            public Node(PlayerLoopTree tree, PlayerLoopSystem playerLoopSystem)
            {
                Tree = tree;
                PlayerLoopSystem = playerLoopSystem;
            }

            public Node(PlayerLoopTree tree, Type type) : this(tree, type, null)
            {
            }

            public Node(PlayerLoopTree tree, Type type, PlayerLoopSystem.UpdateFunction updateDelegate)
            {
                Tree = tree;
                PlayerLoopSystem = new()
                {
                    type = type,
                    updateDelegate = updateDelegate
                };
            }

            public void SetParent(Node parent, int index = int.MaxValue)
            {
                Parent = parent;
                parent.OnSetParent(this, index);
            }

            private void OnSetParent(Node chlid, int index)
            {
                if (index >= SubNodes.Count)
                    SubNodes.Add(chlid);
                else
                    SubNodes.Insert(index, chlid);
                Tree.AddNode(chlid);
            }

            public void AddChild(Node node) => node.SetParent(this);

            /// <summary>
            /// 注意这里会无视掉<see crefILocatedAgentnt.AgentLocation"/>,
            /// 除非你使用<see cref="PlayerLoopTree.AddNode(IAgent)"/>来添加
            /// </summary>
            public void AddChild(IAgent agent) => AddChild(agent.GetPlayerLoopSystem());

            public void AddChild(params IAgent[] agent)
            {
                for (var i = 0; i < agent.Length; i++)
                    AddChild(agent[i].GetPlayerLoopSystem());
            }

            public void AddChild(PlayerLoopSystem system)
            {
                var node = new Node(Tree, system);
                node.SetParent(this);
            }
        }
    }
}