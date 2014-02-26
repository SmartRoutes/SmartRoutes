using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Itinerary
{
    /// <summary>
    /// An itinerary for picking children up.
    /// </summary>
    public class PickUpItineraryModel :ItineraryModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PickUpItineraryModel()
        {

        }

        public override ItineraryType Type
        {
            get
            {
                return ItineraryType.PickUp;
            }
        }
    }
}