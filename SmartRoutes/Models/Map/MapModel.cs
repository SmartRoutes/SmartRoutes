using System.Collections.Generic;

namespace SmartRoutes.Models.Map
{
    public class MapModel
    {
        public AddressLocationModel DepartureAddress { get; set; }
        public AddressLocationModel DestinationAddress { get; set; }
        public IEnumerable<StopTimeLocationModel> Stops { get; set; }
        public IEnumerable<ChildCareTimeLocationModel> ChildCares { get; set; }
    }
}