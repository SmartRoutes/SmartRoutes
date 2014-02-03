using System;
using System.Collections.Generic;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph.Node
{
    public class DestinationNode : IDestinationNode
    {
        public DestinationNode(Destination destination, DateTime time, NodeBase baseNode)
        {
            this.destination = destination;
            BaseNode = baseNode;
            Time = time;
            TimeBackwardNeighbors = new HashSet<INode>();
            TimeForwardNeighbors = new HashSet<INode>();
        }

        public Destination destination { get; private set; }
        public DateTime Time { get; private set; }
        public NodeBase BaseNode { get; private set; }
        public ISet<INode> TimeBackwardNeighbors { get; private set; }
        public ISet<INode> TimeForwardNeighbors { get; private set; }

        // legacy properties
        public string Name
        {
            get { return destination.Name; }
        }

        public double Latitude
        {
            get { return destination.Latitude; }
        }

        public double Longitude
        {
            get { return destination.Longitude; }
        }
    }
}