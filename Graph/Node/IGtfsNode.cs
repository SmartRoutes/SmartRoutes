﻿using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Node
{
    public interface IGtfsNode : INode
    {
        StopTime stopTime { get; }
        int StopId { get; }
        int TripId { get; }
        int Sequence { get; }
        int RouteId { get; }
        int? BlockId { get; }

        IGtfsNode CreateGtfsNode(StopTime stopTime, NodeBase node);
    }
}