﻿using System;
using System.Collections.Generic;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph.Node
{
    public class DestinationNode : IDestinationNode
    {
        public DestinationNode(IDestination destination, DateTime time, NodeBase baseNode)
        {
            BaseNode = baseNode;
            Time = time;
            TimeBackwardNeighbors = new HashSet<INode>();
            TimeForwardNeighbors = new HashSet<INode>();
        }

        public DateTime Time { get; private set; }
        public NodeBase BaseNode { get; private set; }
        public ISet<INode> TimeBackwardNeighbors { get; private set; }
        public ISet<INode> TimeForwardNeighbors { get; private set; }

        // legacy properties
        public string Name
        {
            get { return BaseNode.Name; }
        }

        public double Latitude
        {
            get { return BaseNode.Latitude; }
        }

        public double Longitude
        {
            get { return BaseNode.Longitude; }
        }
    }
}