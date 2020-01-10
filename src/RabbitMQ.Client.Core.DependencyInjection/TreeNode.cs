using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    public class TreeNode
    {
        public string KeyPartition { get; set; }

        public TreeNode Parent { get; set; }

        public List<TreeNode> Nodes { get; } = new List<TreeNode>();
        
        public bool IsLastNode { get; set; }
    }
}