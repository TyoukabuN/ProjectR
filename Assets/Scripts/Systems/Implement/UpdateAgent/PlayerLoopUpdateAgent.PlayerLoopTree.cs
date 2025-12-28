using System.Collections.Generic;
using System.Linq;
using UnityEngine.LowLevel;

namespace PJR.Systems.PlayerLoopUpdateAgent
{
    public partial class PlayerLoopUpdateAgentSystem
    {
        public partial class PlayerLoopTree
        {
            public bool IsInit
            {
                get;
                private set;
            } = false;

            private List<Node> Nodes = new(256);
            
            public Node Root
            {
                get
                {
                    if (Nodes is not { Count: > 0 })
                        return null;
                    return Nodes[0];
                }
            }
            public bool Valid
            {
                get
                {
                    if (!IsInit)
                        return false;
                    return true;
                }
            }

            public void AddNode(Node node)
            {
                if (!Nodes.Contains(node))
                    return;
                Nodes.Add(node);
            }

            public void SetPlayerLoop()
            {
                if(TryOutputPlayerLoopSystem(out var sys))
                    PlayerLoop.SetPlayerLoop(sys);
            }

            public bool TryOutputPlayerLoopSystem(out PlayerLoopSystem sys)
            {
                sys = default;
                if (!IsInit || Root == null)
                    return false;

                sys = Recursion_OutputPlayerLoopSystem(Root);
                return true;
            }
            private PlayerLoopSystem Recursion_OutputPlayerLoopSystem(Node node)
            {
                var sys = node.PlayerLoopSystem;
                if (node.SubNodes is not { Count: > 0 })
                    return sys;
                
                sys.subSystemList = new PlayerLoopSystem[node.SubNodes.Count];
                for (var i = 0; i < node.SubNodes.Count; i++)
                {
                    var subNode = node.SubNodes[i];
                    sys.subSystemList[i] = Recursion_OutputPlayerLoopSystem(subNode);
                }
                return sys;
            }

            public Node AddNode(ILocatedAgent agent) => AddNode(agent.AgentLocation,agent.GetPlayerLoopSystem());
            public void AddNode(AgentLocation location, params PlayerLoopSystem[] systems)
            {
                if (!Valid || location.IsEmpty)
                    return;
                if (!TryGetAnchorLocation(location, out Node parent, out int anchorNodeIndex))
                    return;
                
                var nodes = new Node[systems.Length];
                for (var i = 0; i < systems.Length; i++)
                {
                    nodes[i] = new Node(this, systems[i]);
                }
                if (anchorNodeIndex >= parent.SubNodes.Count)
                    parent.SubNodes.AddRange(nodes);
                else
                    parent.SubNodes.InsertRange(anchorNodeIndex, nodes);
                
                Nodes.AddRange(nodes);
            }
            
            public Node AddNode(AgentLocation location, PlayerLoopSystem system)
            {
                if (!Valid)
                    return null;
                if (!TryGetAnchorLocation(location, out Node parent, out int anchorNodeIndex))
                    return null;
                var node = new Node(this, system);
                node.SetParent(parent, anchorNodeIndex);
                return node;
            }


            /// <summary>
            /// 找到要插入parent:node 和其下subNode的索引
            /// </summary>
            /// <param name="location"></param>
            /// <param name="parent"></param>
            /// <param name="anchorNodeIndex"></param>
            /// <returns></returns>
            private bool TryGetAnchorLocation(AgentLocation location, out Node parent, out int anchorNodeIndex)
            {
                parent = null;
                anchorNodeIndex = -1;
                if (!Valid)
                    return false;
                parent = Nodes.FirstOrDefault(item => location.TargetType == item.PlayerLoopSystem.type);
                if (parent == null)
                    return false;

                if (location.AnchorType == null)
                {
                    if (location.Order == AgentLocation.EOrder.Before)
                        anchorNodeIndex = 0;
                    else if (location.Order == AgentLocation.EOrder.After)
                        anchorNodeIndex = int.MaxValue;
                }
                else
                {
                    anchorNodeIndex = parent.SubNodes.FindIndex(item => location.AnchorType == item.PlayerLoopSystem.type);
                    //有AnchorType但是没有找到对应的索引
                    if (anchorNodeIndex < 0)
                        return false;
                    if (location.Order == AgentLocation.EOrder.Before)
                    {
                    }
                    else if (location.Order == AgentLocation.EOrder.After)
                    {
                        anchorNodeIndex++;
                    }
                }
                
                if (anchorNodeIndex >= parent.SubNodes.Count)
                    anchorNodeIndex = int.MaxValue; //让Node.OnSetParent进Add分支
                return true;
            }


            public static PlayerLoopTree GetTreeByDefaultPlayerLoop()
                => Get(PlayerLoop.GetDefaultPlayerLoop());
            public static PlayerLoopTree GetTreeByCurrentPlayerLoop()
                => Get(PlayerLoop.GetCurrentPlayerLoop());
            public static PlayerLoopTree Get(PlayerLoopSystem sys)
            {
                var tree = new PlayerLoopTree();
                AddNodeRecursion(tree, null, sys);
                tree.IsInit = true;
                return tree;
            }

            private static void AddNodeRecursion(PlayerLoopTree tree, Node parentNode, PlayerLoopSystem sys)
            {
                var node = new Node(tree, sys);
                node.SetParent(parentNode);

                if (sys.subSystemList is not { Length: > 0 })
                    return;
                
                for (int i = 0; i < sys.subSystemList.Length; i++)
                    AddNodeRecursion(tree, node, sys.subSystemList[i]);
            }
        }
    }
}