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
        Stop GetClosestGtfsStop(ILocation location);
        IGtfsNode GetClosestGtfsNode(ILocation location, DateTime time, TimeDirection direction);
        IEnumerable<IGtfsNode> GetDestinationNeighbors(IDestinationNode destinationNode, TimeDirection direction);
    }
}