using System;
using System.Collections.Generic;
using SmartRoutes.Model;

namespace SmartRoutes.Graph.Node
{
    public interface INode : ILocation
    {
        NodeBase BaseNode { get; }
        DateTime Time { get; }
        ISet<INode> TimeBackwardNeighbors { get; }
        ISet<INode> TimeForwardNeighbors { get; }

        // legacy properties, these will redirect to BaseNode properties
        string Name { get; }
    }
}