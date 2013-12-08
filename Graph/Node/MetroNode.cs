using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph.Node
{
    public class MetroNode : IMetroNode
    {
        public DateTime Time { get; private set; }
        public NodeBase BaseNode { get; private set; }
        public int StopID { get; private set; }
        public int TripID { get; private set; }
        public int Sequence { get; private set; }
        public ISet<INode> UpwindNeighbors { get; private set; }
        public ISet<INode> DownwindNeighbors { get; private set; }

        // legacy properties
        public string Name { get { return BaseNode.Name; } }
        public double Latitude { get { return BaseNode.Latitude; } }
        public double Longitude { get { return BaseNode.Longitude; } }

        public MetroNode(StopTime stopTime, NodeBase BaseNode)
        {
            this.BaseNode = BaseNode;
            Time = stopTime.ArrivalTime;
            StopID = stopTime.StopId;
            TripID = stopTime.TripId;
            Sequence = stopTime.Sequence;
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
        }

        public MetroNode()
        {
        }

        // inelegant way to create nodes w/o revealing implementation
        // and without making a million calls to dependency injector
        public IMetroNode CreateMetroNode(StopTime stopTime, NodeBase Node)
        {
            return new MetroNode(stopTime, Node);
        }
    }
}