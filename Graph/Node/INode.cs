using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Sorta;

namespace Graph.Node
{
    public interface INode
    {
        ISet<INode> UpwindNeighbors { get; set; }
        ISet<INode> DownwindNeighbors { get; set; }
        DateTime Time();
        double Latitude();
        double Longitude();
        string Name();
    }
}
