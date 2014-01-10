using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Node
{
    public interface IGtfsNode : INode
    {
        int StopID { get; }
        int TripID { get; }
        int Sequence { get; }

        IGtfsNode CreateGtfsNode(StopTime stopTime, NodeBase Node);
    }
}
