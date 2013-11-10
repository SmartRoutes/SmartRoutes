using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Odjfs;

namespace Graph.Node
{
    public class ChildCareNode : IChildcareNode
    {
        public ISet<INode> UpwindNeighbors { get; set; }
        public ISet<INode> DownwindNeighbors { get; set; }
        public DateTime Time { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Name { get; private set; }

        public ChildCareNode(double Latitude, double Longitude, DateTime Time)
        {
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
            this.Time = Time;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public ChildCareNode() { }
    }
}
