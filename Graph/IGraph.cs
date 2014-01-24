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
        void GetGtfsEntities();
        void GetDestinations();
        INode[] GraphNodes { get; }
        IEnumerable<Stop> closestMetroStops(ILocation location, int numStops);
        IEnumerable<IGtfsNode> closestMetroNodes(ILocation location, DateTime Time, TimeDirection Direction, int numNodes);
        List<IGtfsNode> GetChildCareNeighbors(IDestinationNode childCareNode, TimeDirection Direction);
    }
}