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
        private double _latitude;
        private double _longitude;
        private DateTime _time;

        public ChildCareNode(double latitude, double longitude, DateTime time)
        {
            _latitude = latitude;
            _longitude = longitude;
            _time = time;
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
        }

        public ChildCareNode() { }

        public DateTime Time()
        {
            return _time;
        }

        public double Latitude()
        {
            return _latitude;
        }

        public double Longitude()
        {
            return _longitude;
        }

        public string Name()
        {
            return "what what";
        }
    }
}
