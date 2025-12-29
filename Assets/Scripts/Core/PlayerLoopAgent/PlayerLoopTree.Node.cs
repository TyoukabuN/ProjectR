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
            public Node Parent
            {
                get => _parent;
                set => _parent = value;
            }

            public List<Node> SubNodes = new(64);
            public PlayerLoopSystem PlayerLoopSystem;

            private Node _parent;

            public Node(PlayerLoopSystem playerLoopSystem)
            {
                PlayerLoopSystem = playerLoopSystem;
            }
            public Node(PlayerLoopTree tree, PlayerLoopSystem playerLoopSystem): this(playerLoopSystem)
            {
                SetTree(tree);
            }
            public Node(PlayerLoopTree tree, Type type, PlayerLoopSystem.UpdateFunction updateDelegate)
            {
                SetTree(tree);
                PlayerLoopSystem = new()
                {
                    type = type,
                    updateDelegate = updateDelegate
                };
            }

            public void SetTree(PlayerLoopTree newTree)
            {
                if (Tree == newTree)
                    return;
                
                if(Tree != null)
                    Tree.RemoveNode(this);
                
                Tree = newTree;
                if (newTree != null)
                    newTree.OnAddNode(this);
            }

            public void SetParent(Node newParent, int index = int.MaxValue)
            {
                if(Parent == newParent)
                    return;
                if(Parent != null)
                    Parent.RemoveChild(this);
                
                Parent = newParent;
                if (newParent != null)
                {
                    newParent.OnSetParent(this, index);
                    SetTree(newParent.Tree);
                }
            }

            private void OnSetParent(Node chlid, int index)
            {
                if (index >= SubNodes.Count)
                    SubNodes.Add(chlid);
                else
                    SubNodes.Insert(index, chlid);
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

            public void RemoveChild(Node node)
            {
                if (node == null)
                    return;
                SubNodes?.Remove(node);
            }
        }
    }
}