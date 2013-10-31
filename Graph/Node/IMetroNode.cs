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
        int TripID();
        int Sequence();
        int ShapeID();
        IMetroNode CreateNode(StopTime stopTime);
    }
}
