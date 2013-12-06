using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Test
{
    public class GraphTestImplementation : IGraph
    {
        public void GetSortaEntities()
        {
            throw new NotImplementedException();
        }

        public void GetChildCares()
        {
            throw new NotImplementedException();
        }

        public Node.INode[] GraphNodes { get; set; }
    }

    class NodeTestImplementation : INode
    {
        public ISet<INode> UpwindNeighbors { get; set; }
        public ISet<INode> DownwindNeighbors { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public NodeTestImplementation(string Name, DateTime Time)
        {
            DownwindNeighbors = new HashSet<INode>();
            UpwindNeighbors = new HashSet<INode>();
            this.Name = Name;
            this.Time = Time;
        }
    }

    static class GraphCreationMethods
    {
        public static GraphTestImplementation TwoNodeGraph()
        {
            var NodeA = new NodeTestImplementation("A", new DateTime(1970, 1, 1, 10, 0, 0));
            var NodeB = new NodeTestImplementation("B", new DateTime(1970, 1, 1, 12, 0, 0));

            NodeA.UpwindNeighbors.Add(NodeB);
            NodeB.DownwindNeighbors.Add(NodeA);

            var graph = new GraphTestImplementation();
            graph.GraphNodes = new INode[] { NodeA, NodeB };

            return graph;
        }
    }
}
