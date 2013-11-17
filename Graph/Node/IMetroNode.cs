using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Sorta;

namespace Graph.Node
{
    public interface IMetroNode : INode
    {
        int StopID { get; }
        int TripID { get; }
        int Sequence { get; }

        IMetroNode CreateNode(StopTime stopTime);
    }
}
