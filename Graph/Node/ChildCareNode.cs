using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Odjfs.ChildCares;

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

        public ChildCareNode(ChildCare childcare, DateTime Time)
        {
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
            this.Time = Time;
            this.Name = childcare.Name;
            
            // assume value is not null
            if (childcare.Latitude == null || childcare.Longitude == null)
            {
                throw new ArgumentNullException("Attempt to create childcare node without Latitude / Longitude defined.");
            }

            this.Latitude = childcare.Latitude.Value;
            this.Longitude = childcare.Longitude.Value;
        }
    }
}
