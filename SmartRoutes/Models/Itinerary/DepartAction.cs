using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartRoutes.Model;

namespace SmartRoutes.Models.Itinerary
{
    public class DepartAction : IAddressAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DepartAction()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">The location of the departure address.</param>
        /// <param name="address">The address from which to depart.</param>
        public DepartAction(ILocation location, string address)
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
                return ItineraryActionType.DepartFromAddress;
            }
        }

        /// <summary>
        /// The address from which to depart for this asction.
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// The longitude of the departure location.
        /// </summary>
        public double Latitude
        {
            get; set;
        }

        /// <summary>
        /// The latitude of the departure location.
        /// </summary>
        public double Longitude
        {
            get; set;
        }
    }
}