using System;
using System.Collections.Generic;
using SmartRoutes.Model;

namespace SmartRoutes.Graph.Node
{
    public class DestinationNode : IDestinationNode
    {
        public DestinationNode(IDestination destination, DateTime time, NodeBase baseNode)
        {
            Destination = destination;
            BaseNode = baseNode;
            Time = time;
            TimeBackwardNeighbors = new HashSet<INode>();
            TimeForwardNeighbors = new HashSet<INode>();
        }

        public IDestination Destination { get; private set; }
        public DateTime Time { get; private set; }
        public NodeBase BaseNode { get; private set; }
        public ISet<INode> TimeBackwardNeighbors { get; private set; }
        public ISet<INode> TimeForwardNeighbors { get; private set; }

        // legacy properties
        public string Name
        {
            get { return Destination.Name; }
        }

        public double Latitude
        {
            get { return Destination.Latitude; }
        }

        public double Longitude
        {
            get { return Destination.Longitude; }
        }
    }
}