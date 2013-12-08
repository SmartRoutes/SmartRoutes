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
        public ISet<INode> UpwindNeighbors { get; set; }
        public ISet<INode> DownwindNeighbors { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public NodeTestImplementation(string Name, DateTime Time, double LatLon)
        {
            DownwindNeighbors = new HashSet<INode>();
            UpwindNeighbors = new HashSet<INode>();
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

            NodeA.UpwindNeighbors.Add(NodeB);
            NodeB.DownwindNeighbors.Add(NodeA);

            return new INode[] { NodeA, NodeB };
        }

        public static INode[] ThreeNodesV()
        {
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0), 1);
            var NodeB = new NodeTestImplementation("B", new DateTime(1970, 1, 1, 12, 0, 0), 2);
            var NodeC = new NodeTestImplementation("C", new DateTime(1970, 1, 1, 13, 0, 0), 3);

            NodeA.UpwindNeighbors.Add(NodeB);
            NodeA.UpwindNeighbors.Add(NodeC);
            NodeB.DownwindNeighbors.Add(NodeA);
            NodeC.DownwindNeighbors.Add(NodeA);

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

            NodeA.UpwindNeighbors.Add(NodeB);
            NodeB.DownwindNeighbors.Add(NodeA);
            NodeB.UpwindNeighbors.Add(NodeC);
            NodeC.DownwindNeighbors.Add(NodeB);

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

            NodeA.UpwindNeighbors.Add(NodeB);
            NodeA.UpwindNeighbors.Add(NodeC);
            NodeC.UpwindNeighbors.Add(NodeD);
            NodeD.UpwindNeighbors.Add(NodeE);

            return new INode[] { NodeA, NodeB, NodeC, NodeD, NodeE };
        }
    }
}
