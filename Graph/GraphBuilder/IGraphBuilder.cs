using System.Collections.Generic;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph
{
    public interface IGraphBuilder
    {
        IGraph BuildGraph(IEnumerable<StopTime> stopTimes, IEnumerable<IDestination> destinations, GraphBuilderSettings settings);
    }
}