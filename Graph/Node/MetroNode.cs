using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph.Node
{
    class MetroNode : IMetroNode
    {
        // INode properties
        public ISet<INode> UpwindNeighbors { get; set; }
        public ISet<INode> DownwindNeighbors { get; set; }
        public DateTime Time { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Name { get; private set; }
        public int TripID { get; private set; }
        public int Sequence { get; private set; }

        // MetroNode property
        public int StopID { get; private set; }

        public MetroNode(StopTime stopTime)
        {
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
            Time = stopTime.ArrivalTime;
            Latitude = stopTime.Stop.Latitude;
            Longitude = stopTime.Stop.Longitude;
            Name = stopTime.Stop.Name;
            StopID = stopTime.StopId;
            TripID = stopTime.TripId;
            Sequence = stopTime.Sequence;
        }

        public MetroNode()
        {
        }

        // inelegant way to create nodes w/o revealing implementation
        // and without making 10000000000 calls to dependency injector
        public IMetroNode CreateNode(StopTime stopTime)
        {
            return new MetroNode(stopTime);
        }
    }
}