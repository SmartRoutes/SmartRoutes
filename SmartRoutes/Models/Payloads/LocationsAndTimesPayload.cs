using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    // Payload object for locations and times.
    public class LocationsAndTimesPayload
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LocationsAndTimesPayload()
        {

        }

        public AddressPayload DropOffDepartureAddress
        {
            get;
            set;
        }

        public AddressPayload DropOffDestinationAddress
        {
            get;
            set;
        }

        public DateTime DropOffLatestArrivalTime
        {
            get;
            set;
        }

        /// <summary>
        /// I think the name speaks for itself.
        /// </summary>
        public bool PickUpDepartureAddressSameAsDropOffDestination
        {
            get;
            set;
        }

        public AddressPayload PickUpDepartureAddress
        {
            get;
            set;
        }

        public DateTime PickUpDepartureTime
        {
            get;
            set;
        }

        public AddressPayload PickUpDestinationAddress
        {
            get;
            set;
        }

        public bool PickUpDestinationSameAsDropOffDeparture
        {
            get;
            set;
        }


    }
}