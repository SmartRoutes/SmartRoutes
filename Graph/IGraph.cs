using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.Heap;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph
{
    public interface IGraph
    {
        INode[] GraphNodes { get; }
        IEnumerable<NodeInfo> Search(ILocation StartLocation, ILocation EndLocation,
            DateTime StartTime, TimeDirection Direction, IEnumerable<Func<IDestination, bool>> Criterion);
    }
}