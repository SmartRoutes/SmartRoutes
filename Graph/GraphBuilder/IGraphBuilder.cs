using System.Collections.Generic;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph
{
    public interface IGraphBuilder
    {
        INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<ChildCare> ChildCares);
    }
}