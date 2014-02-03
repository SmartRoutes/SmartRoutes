using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Node
{
    public class GtfsNode : IGtfsNode
    {
        public StopTime stopTime { get; private set; }
        public DateTime Time { get; private set; }
        public NodeBase BaseNode { get; private set; }
        public int StopId { get { return stopTime.StopId; } }
        public int TripId { get { return stopTime.TripId; } }
        public int Sequence { get { return stopTime.Sequence; } }
        public int RouteId { get { return stopTime.Trip.RouteId; } }
        public int? BlockId { get { return stopTime.Trip.BlockId; } }
        public ISet<INode> TimeBackwardNeighbors { get; private set; }
        public ISet<INode> TimeForwardNeighbors { get; private set; }

        // legacy properties
        public string Name { get { return BaseNode.Name; } }
        public double Latitude { get { return BaseNode.Latitude; } }
        public double Longitude { get { return BaseNode.Longitude; } }

        public GtfsNode(StopTime stopTime, NodeBase baseNode)
        {
            this.stopTime = stopTime;
            this.BaseNode = baseNode;
            Time = stopTime.ArrivalTime;
            TimeBackwardNeighbors = new HashSet<INode>();
            TimeForwardNeighbors = new HashSet<INode>();
        }

        public GtfsNode()
        {
        }

        // inelegant way to create nodes w/o revealing implementation
        // and without making a million calls to dependency injector
        public IGtfsNode CreateGtfsNode(StopTime stopTime, NodeBase Node)
        {
            return new GtfsNode(stopTime, Node);
        }
    }
}