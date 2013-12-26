using System.Collections.Generic;

namespace SmartRoutes.Model.Gtfs
{
    public class GtfsCollection : EntityCollection<GtfsArchive>
    {
        public Agency[] Agencies { get; set; }
        public Route[] Routes { get; set; }
        public Service[] Services { get; set; }
        public ServiceException[] ServiceExceptions { get; set; }
        public Shape[] Shapes { get; set; }
        public ShapePoint[] ShapePoints { get; set; }
        public Stop[] Stops { get; set; }
        public StopTime[] StopTimes { get; set; }
        public Trip[] Trips { get; set; }
        public Block[] Blocks { get; set; }
    }
}