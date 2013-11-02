using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Sorta;

namespace Graph.Node
{
    class MetroNode : IMetroNode
    {
        public ISet<INode> UpwindNeighbors { get; set; }
        public ISet<INode> DownwindNeighbors { get; set; }
        private StopTime _stopTime;

        public MetroNode(StopTime stopTime)
        {
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
            _stopTime = stopTime;
        }

        public IMetroNode CreateNode(StopTime stopTime)
        {
            return new MetroNode(stopTime);
        }

        public int TripID()
        {
            return _stopTime.Trip.Id;
        }

        public int Sequence()
        {
            return _stopTime.Sequence;
        }

        public int ShapeID()
        {
            return _stopTime.Trip.Shape.Id;
        }

        public DateTime Time()
        {
            return _stopTime.ArrivalTime;
        }

        public double Latitude()
        {
            return _stopTime.Stop.Latitude;
        }

        public double Longitude()
        {
            return _stopTime.Stop.Longitude;
        }

        public string Name()
        {
            return _stopTime.Stop.Name;
        }
    }
}