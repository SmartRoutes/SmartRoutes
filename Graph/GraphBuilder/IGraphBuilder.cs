using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.SortaScraper.Support;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.Graph
{
    public interface IGraphBuilder
    {
        INode[] BuildGraph(EntityCollection Collection, IEnumerable<ChildCare> ChildCares);
    }
}
