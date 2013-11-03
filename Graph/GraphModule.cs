﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using Graph.Node;

namespace Graph
{
    public class GraphModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMetroNode>().To<MetroNode>();
            Bind<IChildcareNode>().To<ChildCareNode>();
            Bind<IGraphBuilder>().To<GraphBuilder>();
            Bind<IGraph>().To<Graph>();
        }
    }
}