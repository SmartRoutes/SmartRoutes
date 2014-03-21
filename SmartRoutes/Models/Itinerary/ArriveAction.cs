using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartRoutes.Model;

namespace SmartRoutes.Models.Itinerary
{
    public class ArriveAction : IItineraryAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ArriveAction()
        {

        }
        
        /// <summary>
        /// Convenience constructor.
        /// </summary>
        /// <param name="location">The location of the arrival address.</param>
        /// <param name="address">The address to arrive at.</param>
        public ArriveAction(ILocation location, string address)
        {
            this.Latitude = location.Latitude;
            this.Longitude = location.Longitude;
            this.Address = address;
        }

        /// <summary>
        /// The type of action (used for serialization).
        /// </summary>
        public ItineraryActionType Action
        {
            get
            {
                return ItineraryActionType.ArriveAtAddress;
            }
        }

        /// <summary>
        /// The address to arrived at.
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// The latitude of the arrival location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude of the arrival location.
        /// </summary>
        public double Longitude { get; set; }
    }
}