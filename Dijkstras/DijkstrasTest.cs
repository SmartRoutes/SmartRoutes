using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Test
{
    [TestClass]
    public class DijkstrasTest
    {
        [TestMethod]
        public void TestTwoNodeGraph()
        {
            var graph = GraphCreationMethods.TwoNodeGraph();
            var StartNodes = new INode[] { graph.GraphNodes[0] };
            var result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => x.Name == "B",
                SmartRoutes.Graph.Direction.Upwind);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("B", result[0].node.Name);
            Assert.AreEqual("A", result[0].parent.node.Name);
            Assert.IsNull(result[0].parent.parent);
        }
    }
}
