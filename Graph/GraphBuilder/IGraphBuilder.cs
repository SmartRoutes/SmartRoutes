using System.Collections.Generic;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph
{
    public interface IGraphBuilder
    {
        GraphBuilderSettings Settings { get; }
        Dictionary<int, List<int>> StopToNearest { get; }
        Dictionary<int, List<IMetroNode>> StopToNodes { get; }
        Dictionary<int, List<int>> ChildCareToStops { get; }
        INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<ChildCare> ChildCares);
    }
}