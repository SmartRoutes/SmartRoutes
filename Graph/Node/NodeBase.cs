using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph.Node
{
    public class NodeBase : ILocation
    {
        public string Name { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public NodeBase(string Name, double Latitude, double Longitude)
        {
            this.Name = Name;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }
    }
}
