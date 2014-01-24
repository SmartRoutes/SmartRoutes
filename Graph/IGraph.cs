using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.Heap;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph
{
    public interface IGraph
    {
        INode[] GraphNodes { get; }
        IEnumerable<Stop> GetClosestGtfsStops(ILocation location, int numStops);
        IEnumerable<IGtfsNode> GetClosestGtfsNodes(ILocation location, DateTime time, TimeDirection direction, int numNodes);
        IEnumerable<IGtfsNode> GetDestinationNeighbors(IDestinationNode destinationNode, TimeDirection direction);
    }
}