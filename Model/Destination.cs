using System.Collections.Generic;

namespace SmartRoutes.Model
{
    public class Destination : ILocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}