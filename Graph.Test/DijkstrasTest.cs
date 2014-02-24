using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Test
{
    // TODO: fix these tests
    /* [TestClass]
    public class DijkstrasTest
    {
        [TestMethod]
        public void Dijkstras0()
        {
            // empty starting set
            var StartNodes = new INode[] { };

            var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => false,
                SmartRoutes.Graph.TimeDirection.Forwards);

            Assert.AreEqual(0, Result.Count);
        }

        [TestMethod]
        public void Dijkstras1a()
        {
            // one node graph, starting node is not goal node
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0), 1);
            var StartNodes = new INode[] { NodeA };

            var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => false,
                SmartRoutes.Graph.TimeDirection.Forwards);

            Assert.AreEqual(0, Result.Count);
        }

        [TestMethod]
        public void Dijkstras1b()
        {
            // one node graph, starting node is goal node
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0), 1);
            var StartNodes = new INode[] { NodeA };

            var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => x.Name == "A",
                SmartRoutes.Graph.TimeDirection.Forwards);

            Assert.AreEqual(1, Result.Count);
            Assert.AreEqual("A", Result[0].node.Name);
            Assert.IsNull(Result[0].parent);
        }

        [TestMethod]
        public void Dijkstras2a()
        {
            // two node graph, connected, second node is goal node
            var Nodes = GraphCreationMethods.TwoConnectedNodes();
            var TimeForwardStartNodes = new INode[] { Nodes[0] };
            var TimeBackwardStartNodes = new INode[] { Nodes[1] };

            var ForwardResult = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                TimeForwardStartNodes,
                x => x.Name == "B",
                SmartRoutes.Graph.TimeDirection.Forwards);

            var BackwardResult = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                TimeBackwardStartNodes,
                x => x.Name == "A",
                SmartRoutes.Graph.TimeDirection.Backwards);

            Assert.AreEqual(1, ForwardResult.Count);
            Assert.AreEqual("B", ForwardResult[0].node.Name);
            Assert.AreEqual("A", ForwardResult[0].parent.node.Name);
            Assert.IsNull(ForwardResult[0].parent.parent);

            Assert.AreEqual(1, BackwardResult.Count);
            Assert.AreEqual("A", BackwardResult[0].node.Name);
            Assert.AreEqual("B", BackwardResult[0].parent.node.Name);
            Assert.IsNull(BackwardResult[0].parent.parent);
        }

        [TestMethod]
        public void Dijkstras2b()
        {
            // two node graph, connected, first node is goal node
            var Nodes = GraphCreationMethods.TwoConnectedNodes();
            var TimeForwardStartNodes = new INode[] { Nodes[0] };
            var TimeBackwardStartNodes = new INode[] { Nodes[1] };

            var ForwardResult = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                TimeForwardStartNodes,
                x => x.Name == "A",
                SmartRoutes.Graph.TimeDirection.Forwards);

            var BackwardResult = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                TimeBackwardStartNodes,
                x => x.Name == "B",
                SmartRoutes.Graph.TimeDirection.Backwards);

            Assert.AreEqual(1, ForwardResult.Count);
            Assert.AreEqual("A", ForwardResult[0].node.Name);
            Assert.IsNull(ForwardResult[0].parent);

            Assert.AreEqual(1, BackwardResult.Count);
            Assert.AreEqual("B", BackwardResult[0].node.Name);
            Assert.IsNull(BackwardResult[0].parent);
        }

        [TestMethod]
        public void Dijkstras2c()
        {
            // two node graph, both in starting set
            var Nodes = GraphCreationMethods.TwoConnectedNodes();
            var TimeForwardStartNodes = Nodes;
            var TimeBackwardStartNodes = Nodes;

            var ForwardResult = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                TimeForwardStartNodes,
                x => x.Name == "A",
                SmartRoutes.Graph.TimeDirection.Forwards);

            var BackwardResult = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                TimeBackwardStartNodes,
                x => x.Name == "B",
                SmartRoutes.Graph.TimeDirection.Backwards);

            Assert.AreEqual(1, ForwardResult.Count);
            Assert.AreEqual("A", ForwardResult[0].node.Name);
            Assert.IsNull(ForwardResult[0].parent);

            Assert.AreEqual(1, BackwardResult.Count);
            Assert.AreEqual("B", BackwardResult[0].node.Name);
            Assert.IsNull(BackwardResult[0].parent);
        }

        [TestMethod]
        public void Dijkstras3a()
        {
            // three node graph, V, "B" and "C" are both goals, "A" to "B" has least cost
            var Nodes = GraphCreationMethods.ThreeNodesV();
            var StartNodes = new INode[] { Nodes[0] };

            var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => x.Name == "B" || x.Name == "C",
                SmartRoutes.Graph.TimeDirection.Forwards);

            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual("B", Result[0].node.Name);
            Assert.AreEqual("A", Result[0].parent.node.Name);
            Assert.IsNull(Result[0].parent.parent);

            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual("C", Result[1].node.Name);
            Assert.AreEqual("A", Result[1].parent.node.Name);
            Assert.IsNull(Result[1].parent.parent);
        }

        [TestMethod]
        public void Dijkstras3b()
        {
            // three node graph, V, "B" and "C" are both goals, "A" to "C" has least cost
            var Nodes = GraphCreationMethods.ThreeNodesV();
            ((NodeTestImplementation)Nodes[1]).Time = new DateTime(1970, 1, 1, 14, 0, 0);
            var StartNodes = new INode[] { Nodes[0] };

            var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => x.Name == "B" || x.Name == "C",
                SmartRoutes.Graph.TimeDirection.Forwards);

            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual("C", Result[0].node.Name);
            Assert.AreEqual("A", Result[0].parent.node.Name);
            Assert.IsNull(Result[0].parent.parent);

            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual("B", Result[1].node.Name);
            Assert.AreEqual("A", Result[1].parent.node.Name);
            Assert.IsNull(Result[1].parent.parent);
        }

        [TestMethod]
        public void Dijkstras3c()
        {
            // three node graph, goal path "A" -> "B" -> "C"
            var Nodes = GraphCreationMethods.ThreeNodesLine();
            var StartNodes = new INode[] { Nodes[0] };

            var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
                StartNodes,
                x => x.Name == "C",
                SmartRoutes.Graph.TimeDirection.Forwards);

            Assert.AreEqual(1, Result.Count);
            Assert.AreEqual("C", Result[0].node.Name);
            Assert.AreEqual("B", Result[0].parent.node.Name);
            Assert.AreEqual("A", Result[0].parent.parent.node.Name);
            Assert.IsNull(Result[0].parent.parent.parent);
        }

        //[TestMethod]
        //public void Dijkstras5()
        //{
        //    // Start node "A", Goal nodes are "B" and "E", duplicate locations so "E"
        //    // should be only return since it is faster
        //    var Nodes = GraphCreationMethods.FiveNodes();
        //    var StartNodes = new INode[] { Nodes[0] };

        //    var Result = SmartRoutes.Graph.ExtensionMethods.Dijkstras(
        //        StartNodes,
        //        x => x.Name == "B" || x.Name == "E",
        //        SmartRoutes.GrapTimeDirectionoBackwardsnd);

        //    Assert.AreEqual(1, Result.Count);
        //    Assert.AreEqual("E", Result[0].node.Name);
        //    Assert.AreEqual("D", Result[0].parent.node.Name);
        //    Assert.AreEqual("C", Result[0].parent.parent.node.Name);
        //    Assert.AreEqual("A", Result[0].parent.parent.parent.node.Name);
        //    Assert.IsNull(Result[0].parent.parent.parent.parent);
        //}
    }
    */
}
