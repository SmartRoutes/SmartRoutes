using System.Collections.Generic;
using Model.Sorta;

namespace SortaScraper.Support
{
    public class EntityCollection
    {
        public Archive Archive { get; set; }
        public bool ContainsEntities { get; set; }
        public IEnumerable<Agency> Agencies { get; set; }
        public IEnumerable<Route> Routes { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<ServiceException> ServiceException { get; set; }
        public IEnumerable<Shape> Shapes { get; set; }
        public IEnumerable<ShapePoint> ShapePoints { get; set; }
        public IEnumerable<Stop> Stops { get; set; }
        public IEnumerable<StopTime> StopTimes { get; set; }
        public IEnumerable<Trip> Trips { get; set; }
        public IEnumerable<Block> Blocks { get; set; }
    }
}