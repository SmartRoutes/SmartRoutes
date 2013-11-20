using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortaScraper.Support;
using Graph.Node;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace Graph
{
    public interface IGraphBuilder
    {
        INode[] BuildGraph(EntityCollection Collection, IEnumerable<ChildCare> ChildCares);
    }
}
