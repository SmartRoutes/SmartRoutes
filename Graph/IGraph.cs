using System;
using System.Collections.Generic;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    public enum TimeDirection
    {
        Backwards,
        Forwards
    }

    public interface IGraph
    {
        INode[] GraphNodes { get; }

        IEnumerable<SearchResult> Search(ILocation startLocation, ILocation endLocation,
            DateTime startTime, TimeDirection direction, IEnumerable<Func<IDestination, bool>> criteria);
    }
}