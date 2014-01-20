using System.Collections.Generic;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph
{
    public interface IGraphBuilder
    {
        GraphBuilderSettings Settings { get; }
        Dictionary<int, List<IGtfsNode>> StopToNodes { get; }
        INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<Destination> Destinations, GraphBuilderSettings Settings);
    }
}