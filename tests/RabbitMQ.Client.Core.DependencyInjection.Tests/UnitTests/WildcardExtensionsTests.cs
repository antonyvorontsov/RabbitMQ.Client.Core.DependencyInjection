using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class WildcardExtensionsTests
    {
        readonly string[] _routes;

        public WildcardExtensionsTests()
        {
            _routes = new[] {
                "#",
                "#.delete",
                "#.create",
                "#.update",
                "create.*",
                "create.#",
                "*.update",
                "*.create.*",
                "*.*.*",
                "*.*.create",
            };
        }

        [Fact]
        public void ShouldProperlyConstructTree()
        {
            var tree = WildcardExtensions.ConstructTree(_routes);
            
            var countNodes = CountNodes(tree);
            Assert.Equal(15, countNodes);
            
            Assert.Equal(4, tree.Count());
            Assert.Contains(tree, x => x.IsLastNode && x.KeyPartition == "#");
            Assert.Contains(tree, x => !x.IsLastNode && x.KeyPartition == "#");
            Assert.Contains(tree, x => x.KeyPartition == "*");
            Assert.Contains(tree, x => x.KeyPartition == "create");

            var sharpNodes = tree.FirstOrDefault(x => !x.IsLastNode && x.KeyPartition == "#").Nodes;
            Assert.Equal(3, sharpNodes.Count);
            Assert.Contains(sharpNodes, x => x.KeyPartition == "delete");
            Assert.Contains(sharpNodes, x => x.KeyPartition == "create");
            Assert.Contains(sharpNodes, x => x.KeyPartition == "update");
            
            var createNodes = tree.FirstOrDefault(x => x.KeyPartition == "create").Nodes;
            Assert.Equal(2, createNodes.Count);
            Assert.Contains(createNodes, x => x.KeyPartition == "*");
            Assert.Contains(createNodes, x => x.KeyPartition == "#");
            
            var asteriskNodes = tree.FirstOrDefault(x => x.KeyPartition == "*").Nodes;
            Assert.Equal(3, asteriskNodes.Count);
            Assert.Contains(asteriskNodes, x => x.KeyPartition == "*");
            Assert.Contains(asteriskNodes, x => x.KeyPartition == "create");
            Assert.Contains(asteriskNodes, x => x.KeyPartition == "update");
            
            var doubleAsteriskNodes = asteriskNodes.FirstOrDefault(x => x.KeyPartition == "*").Nodes;
            Assert.Equal(2, doubleAsteriskNodes.Count);
            Assert.Contains(doubleAsteriskNodes, x => x.KeyPartition == "*");
            Assert.Contains(doubleAsteriskNodes, x => x.KeyPartition == "create");
            
            var asteriskCreateNodes = asteriskNodes.FirstOrDefault(x => x.KeyPartition == "create").Nodes;
            Assert.Single(asteriskCreateNodes);
            Assert.Contains(asteriskCreateNodes, x => x.KeyPartition == "*");
        }

        [Theory]
        [InlineData("create.connection", new[] { "#", "create.*", "create.#" })]
        [InlineData("create.stable.connection", new[] { "#", "create.#", "*.*.*" })]
        [InlineData("connection.create.stable", new[] { "#", "*.create.*", "*.*.*" })]
        [InlineData("file.delete", new[] { "#", "#.delete" })]
        [InlineData("file.info.delete", new[] { "#", "#.delete", "*.*.*" })]
        [InlineData("file.update", new[] { "#", "#.update", "*.update" })]
        [InlineData("file.update.author", new[] { "#", "*.*.*" })]
        [InlineData("file.update.author.credentials", new[] { "#" })]
        [InlineData("report.create", new[] { "#", "#.create" })]
        [InlineData("final.report.create", new[] { "#", "#.create", "*.*.*", "*.*.create" })]
        public void ShouldProperlyGetMatchingRoutes(string routingKey, IEnumerable<string> routes)
        {
            var tree = WildcardExtensions.ConstructTree(_routes);

            var routingKeyParts = routingKey.Split(".");
            var matchingRoutes = WildcardExtensions.GetMatchingRoutePatterns(tree, routingKeyParts).ToList();
            
            Assert.Equal(routes.Count(), matchingRoutes.Count);
            foreach (var route in routes)
            {
                Assert.Contains(matchingRoutes, x => x == route);
            }
        }

        int CountNodes(IEnumerable<TreeNode> nodes)
        {
            var count = nodes.Count();
            foreach (var node in nodes)
            {
                if (node.Nodes.Any())
                {
                    count += CountNodes(node.Nodes);
                }
            }

            return count;
        }
    }
}