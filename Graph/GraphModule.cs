using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using SmartRoutes.Graph.Node;
using SmartRoutes.Heap;

namespace SmartRoutes.Graph
{
    public class GraphModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IGtfsNode>().To<GtfsNode>();
            Bind<IDestinationNode>().To<DestinationNode>();
            Bind<IGraphBuilder>().To<GraphBuilder>();
            Bind<IGraph>().To<Graph>();
            Bind<IFibonacciHeap<INode, TimeSpan>>()
                .To<FibonacciHeap<INode, TimeSpan>>();
        }
    }
}
