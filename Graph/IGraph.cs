using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.Heap;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    public interface IGraph
    {
        void GetSortaEntities();
        void GetChildCares();
        INode[] GraphNodes { get; }
        IMetroNode closestMetroNode(ILocation location, DateTime Time, TimeDirection Direction);
    }
}