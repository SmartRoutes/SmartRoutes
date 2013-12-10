using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Test
{
    class NodeTestImplementation : INode
    {
        public NodeBase BaseNode { get; set; }
        public ISet<INode> TimeForwardNeighbors { get; set; }
        public ISet<INode> TimeBackwardNeighbors { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public NodeTestImplementation(string Name, DateTime Time, double LatLon)
        {
            TimeBackwardNeighbors = new HashSet<INode>();
            TimeForwardNeighbors = new HashSet<INode>();
            this.Name = Name;
            this.Time = Time;
            Latitude = LatLon;
            Longitude = LatLon;
            BaseNode = new NodeBase(Name, Latitude, Longitude);
        }
    }

    static class GraphCreationMethods
    {
        public static INode[] TwoConnectedNodes()
        {
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0), 1);
            var NodeB = new NodeTestImplementation("B", new DateTime(1970, 1, 1, 12, 0, 0), 2);

            NodeA.TimeForwardNeighbors.Add(NodeB);
            NodeB.TimeBackwardNeighbors.Add(NodeA);

            return new INode[] { NodeA, NodeB };
        }

        public static INode[] ThreeNodesV()
        {
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0), 1);
            var NodeB = new NodeTestImplementation("B", new DateTime(1970, 1, 1, 12, 0, 0), 2);
            var NodeC = new NodeTestImplementation("C", new DateTime(1970, 1, 1, 13, 0, 0), 3);

            NodeA.TimeForwardNeighbors.Add(NodeB);
            NodeA.TimeForwardNeighbors.Add(NodeC);
            NodeB.TimeBackwardNeighbors.Add(NodeA);
            NodeC.TimeBackwardNeighbors.Add(NodeA);

            return new INode[] { NodeA, NodeB, NodeC };
        }

        public static INode[] ThreeNodesLine()
        {
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0), 1);
            NodeA.Latitude = 1; NodeA.Longitude = 1;
            var NodeB = new NodeTestImplementation("B", new DateTime(1970, 1, 1, 15, 0, 0), 2);
            NodeB.Latitude = 2; NodeB.Longitude = 2;
            var NodeC = new NodeTestImplementation("C", new DateTime(1970, 1, 1, 13, 0, 0), 3);
            NodeC.Latitude = 3; NodeC.Longitude = 3;

            NodeA.TimeForwardNeighbors.Add(NodeB);
            NodeB.TimeBackwardNeighbors.Add(NodeA);
            NodeB.TimeForwardNeighbors.Add(NodeC);
            NodeC.TimeBackwardNeighbors.Add(NodeB);

            return new INode[] { NodeA, NodeB, NodeC };
        }

        public static INode[] FiveNodes()
        {
            // Start node NodeA, Goal nodes are NodeB and NodeE, duplicate locations so NodeB
            // should be only return
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 1, 0, 0), 1);
            var NodeB = new NodeTestImplementation("B", new DateTime(1970, 1, 1, 11, 0, 0), 2);
            var NodeC = new NodeTestImplementation("C", new DateTime(1970, 1, 1, 2, 0, 0), 3);
            var NodeD = new NodeTestImplementation("D", new DateTime(1970, 1, 1, 3, 0, 0), 4);
            var NodeE = new NodeTestImplementation("E", new DateTime(1970, 1, 1, 4, 0, 0), 2);

            NodeA.TimeForwardNeighbors.Add(NodeB);
            NodeA.TimeForwardNeighbors.Add(NodeC);
            NodeC.TimeForwardNeighbors.Add(NodeD);
            NodeD.TimeForwardNeighbors.Add(NodeE);

            return new INode[] { NodeA, NodeB, NodeC, NodeD, NodeE };
        }
    }
}
