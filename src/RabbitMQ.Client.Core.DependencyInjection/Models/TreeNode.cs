using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A model that represents nodes supposed for building a tree (trie) structure for a pattern (wildcard) matching.
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Part of the routing key.
        /// </summary>
        /// <remarks>
        /// Routing key split into parts by the dot.
        /// </remarks>
        public string KeyPartition { get; set; } = string.Empty;

        /// <summary>
        /// Parent node.
        /// </summary>
        public TreeNode? Parent { get; set; }

        /// <summary>
        /// Child nodes.
        /// </summary>
        public List<TreeNode> Nodes { get; } = new List<TreeNode>();

        /// <summary>
        /// Flag is the node "last" - a single word that comes without any other parts.
        /// </summary>
        public bool IsLastNode { get; set; }
    }
}