using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortaScraper.Support;
using Graph.Node;

namespace Graph
{
    interface IGraphBuilder
    {
        INode[] BuildGraph(EntityCollection collection);
    }
}
