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
        Stop closestMetroStop(ILocation location);
        IGtfsNode closestMetroNode(ILocation location, DateTime Time, TimeDirection Direction);
        List<IGtfsNode> GetChildCareNeighbors(IDestinationNode childCareNode, TimeDirection Direction);
    }
}