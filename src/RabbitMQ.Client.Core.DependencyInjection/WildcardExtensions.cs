using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    public static class WildcardExtensions
    {
        private const string Separator = ".";
        private const string SingleWordPattern = "*";
        private const string MultipleWordsPattern = "#";
        
        public static IEnumerable<TreeNode> ConstructTree(IEnumerable<string> routingKeyBindings)
        {
            var tree = new List<TreeNode>();

            foreach (var binding in routingKeyBindings)
            {
                var keyParts = binding.Split(Separator);
                TreeNode parentTreeNode = null;
                var currentTreeNode = tree;
                for (var index = 0; index < keyParts.Length; index++)
                {
                    var part = keyParts[index];

                    var existingNode = index == keyParts.Length - 1 
                        ? currentTreeNode.FirstOrDefault(x => x.KeyPartition == part && x.IsLastNode) 
                        : currentTreeNode.FirstOrDefault(x => x.KeyPartition == part && !x.IsLastNode);

                    if (existingNode is null)
                    {
                        var node = new TreeNode
                        {
                            Parent = parentTreeNode,
                            KeyPartition = part,
                            IsLastNode = index == keyParts.Length - 1
                        };
                        currentTreeNode.Add(node);
                        currentTreeNode = node.Nodes;
                        parentTreeNode = node;
                    }
                    else
                    {
                        currentTreeNode = existingNode.Nodes;
                        parentTreeNode = existingNode;
                    }
                }
            }

            return tree;
        }

        public static IEnumerable<string> GetMatchingRoutePatterns(IEnumerable<TreeNode> bindingsTree, string[] routingKeyParts, int depth = 0)
        {
            foreach (var node in bindingsTree)
            {
                var matchingPart = routingKeyParts[depth];
                if (node.KeyPartition == MultipleWordsPattern)
                {
                    if (!node.Nodes.Any())
                    {
                        yield return CollectRoutingKeyInReverseOrder(node);
                    }
                    else
                    {
                        var tails = CollectRoutingKeyTails(routingKeyParts, depth);
                        foreach (var tail in tails)
                        {
                            var routes = GetMatchingRoutePatterns(node.Nodes, tail, depth: 0);
                            foreach (var route in routes)
                            {
                                yield return route;
                            }
                        }
                    }
                }
                else if(node.KeyPartition == SingleWordPattern || node.KeyPartition == matchingPart)
                {
                    if (routingKeyParts.Length == depth + 1 && !node.Nodes.Any())
                    {
                        yield return CollectRoutingKeyInReverseOrder(node);
                    }
                    else if (routingKeyParts.Length != depth + 1 && node.Nodes.Any())
                    {
                        var routes = GetMatchingRoutePatterns(node.Nodes, routingKeyParts, depth + 1).ToList();
                        foreach (var route in routes)
                        {
                            yield return route;
                        }
                    }
                }
            }
        }
        
        private static IEnumerable<string[]> CollectRoutingKeyTails(IReadOnlyCollection<string> routingKeyParts, int depthStart)
        {
            for (var index = depthStart; index < routingKeyParts.Count; index++)
            {
                yield return routingKeyParts.Skip(index).ToArray();
            }
        }
        
        private static string CollectRoutingKeyInReverseOrder(TreeNode node, string routingKey = "")
        {
            routingKey = string.IsNullOrEmpty(routingKey) ? node.KeyPartition : $"{node.KeyPartition}.{routingKey}";
            return node.Parent != null ? CollectRoutingKeyInReverseOrder(node.Parent, routingKey) : routingKey;
        }
    }
}