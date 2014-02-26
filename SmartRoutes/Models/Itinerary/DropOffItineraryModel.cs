using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Itinerary
{
    /// <summary>
    /// An itinerary for dropping off children.
    /// </summary>
    public class DropOffItineraryModel : ItineraryModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropOffItineraryModel()
        {

        }

        /// <summary>
        /// The type of the itinerary model.  Used for serialization.
        /// </summary>
        public override ItineraryType Type
        {
            get
            {
                return ItineraryType.DropOff;
            }
        }
    }
}