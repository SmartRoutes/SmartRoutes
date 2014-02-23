using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        /// <param name="address">The address to arrive at.</param>
        public ArriveAction(string address)
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
    }
}