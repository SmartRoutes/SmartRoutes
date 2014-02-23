using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Itinerary
{
    public class DepartAction : IItineraryAction
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
        /// <param name="address">The address from which to depart.</param>
        public DepartAction(string address)
        {
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
    }
}