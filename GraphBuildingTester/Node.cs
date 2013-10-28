using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Sorta;
using Ninject.Extensions.Factory;

namespace GraphBuildingTester
{
    class Node : INode
    {
        public double PathCost { get; set; }
        public INode Parent { get; set; }
        public IEnumerable<INode> Children { get; set; }
        public IEnumerable<INode> UpwindNeighbors { get; set; }
        public IEnumerable<INode> DownwindNeighbors { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long TripID { get; set; }
        public long Sequence { get; set; }
        public DateTime Time { get; set; }

        public bool IsLessThan(INode node)
        {
            return this.PathCost < node.PathCost;
        }

        public Node()
        {
        }

        public Node(StopTime stopTime)
        {
            PathCost = 0;
            Name = stopTime.Stop.Name;
            Latitude = stopTime.Stop.Latitude;
            Longitude = stopTime.Stop.Longitude;
            Time = stopTime.DepartureTime;
            TripID = stopTime.TripId;
            Sequence = stopTime.Sequence;
        }

        public INode CreateNode(StopTime stopTime)
        {
            return new Node(stopTime);
        }

        public void addUpwindNeighbor(INode node)
        {
            if (UpwindNeighbors == null)
            {
                UpwindNeighbors = new[] { node };
            }
            else
            {
                UpwindNeighbors.Concat<INode>(new[] { node });
            }
        }

        public void addDownwindNeighbor(INode node)
        {
            if (DownwindNeighbors == null)
            {
                DownwindNeighbors = new[] { node };
            }
            else
            {
                DownwindNeighbors.Concat<INode>(new[] { node });
            }
        }
    }
}