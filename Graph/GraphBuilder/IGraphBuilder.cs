﻿using System.Collections.Generic;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph
{
    public interface IGraphBuilder
    {
        IGraph BuildGraph(IEnumerable<StopTime> stopTimes, IEnumerable<Destination> destinations, GraphBuilderSettings settings);
    }
}