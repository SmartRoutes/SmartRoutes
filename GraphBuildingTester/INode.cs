using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Sorta;

namespace GraphBuildingTester
{
    public interface INode
    {
        INode Parent { get; set; }
        IEnumerable<INode> Children { get; set; }
        IEnumerable<INode> UpwindNeighbors { get; set; }
        IEnumerable<INode> DownwindNeighbors { get; set; }
        double PathCost { get; set; }
        long TripID { get; set; }
        long Sequence { get; set; }

        bool IsLessThan(INode node);
        void addUpwindNeighbor(INode node);
        void addDownwindNeighbor(INode node);
        INode CreateNode(StopTime stopTime);
    }
}
