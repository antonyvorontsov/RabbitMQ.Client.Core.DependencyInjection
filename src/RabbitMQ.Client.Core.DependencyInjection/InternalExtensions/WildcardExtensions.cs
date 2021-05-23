using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RabbitMQ.Client.Core.DependencyInjection.Models;

[assembly:InternalsVisibleTo("RabbitMQ.Client.Core.DependencyInjection.Tests")]
namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions
{
    /// <summary>
    /// An extension class that contains functionality of pattern (wildcard) matching.
    /// </summary>
    /// <remarks>
    /// Methods of that class allows finding route patterns by which message handlers are "listening" for messages.
    /// Public access modifier set for this class due to unit testing.
    /// </remarks>
    internal static class WildcardExtensions
    {
        // Change the string "." to a char to solve the compatibility issue in .Net Standard 2.0
        private const char Separator = '.';
        private const string SingleWordPattern = "*";
        private const string MultipleWordsPattern = "#";

        /// <summary>
        /// Construct tree (trie) structure of message handler route patterns.
        /// </summary>
        /// <param name="routePatterns">
        /// Collection of message handler route patterns, which are used by them for a message "listening".
        /// </param>
        /// <returns>
        /// Collection of tree nodes <see cref="TreeNode"/>.
        /// Depending on routing key bindings that collection can be flat or treelike.
        /// </returns>
        public static IEnumerable<TreeNode> ConstructRoutesTree(IEnumerable<string> routePatterns)
        {
            var tree = new List<TreeNode>();

            foreach (var binding in routePatterns)
            {
                var keyParts = binding.Split(Separator);
                TreeNode? parentTreeNode = null;
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

        /// <summary>
        /// Get route patterns that match the given routing key.
        /// </summary>
        /// <param name="tree">Collection (tree, trie) of nodes.</param>
        /// <param name="routingKeyParts">Array of routing key parts split by dots.</param>
        /// <returns>Collection of route patterns that correspond to the given routing key.</returns>
        public static IEnumerable<string> GetMatchingRoutePatterns(IEnumerable<TreeNode> tree, string[] routingKeyParts)
        {
            return GetMatchingRoutePatterns(tree, routingKeyParts, depth: 0);
        }

        private static IEnumerable<string> GetMatchingRoutePatterns(IEnumerable<TreeNode> tree, IReadOnlyList<string> routingKeyParts, int depth)
        {
            foreach (var node in tree)
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
                    if (routingKeyParts.Count == depth + 1 && !node.Nodes.Any())
                    {
                        yield return CollectRoutingKeyInReverseOrder(node);
                    }
                    else if (routingKeyParts.Count != depth + 1 && node.Nodes.Any())
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