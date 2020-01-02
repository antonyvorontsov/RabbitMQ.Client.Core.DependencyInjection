using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    internal class TreeNode
    {
        public string KeyPartition { get; set; }

        public TreeNode Parent { get; set; }

        public List<TreeNode> Nodes { get; set; } = new List<TreeNode>();
        
        public bool IsLastNode { get; set; }
    }
}