using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Node
{
    public interface IGtfsNode : INode
    {
        int StopId { get; }
        int TripId { get; }
        int Sequence { get; }

        IGtfsNode CreateGtfsNode(StopTime stopTime, NodeBase node);
    }
}