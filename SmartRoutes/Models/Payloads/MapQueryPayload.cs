using System.Collections.Generic;

namespace SmartRoutes.Models.Payloads
{
    public class MapQueryPayload
    {
        public AddressPayload DepartureAddress { get; set; }
        public AddressPayload DestinationAddress { get; set; }
        public IEnumerable<int> StopTimeIds { get; set; }
        public IEnumerable<int> ChildCareIds { get; set; }
    }
}