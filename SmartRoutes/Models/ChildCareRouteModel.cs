using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartRoutes.Models.Itinerary;

namespace SmartRoutes.Models
{
    /// <summary>
    /// This object represents the route that a parent
    /// would take.
    /// </summary>
    public class ChildCareRouteModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChildCareRouteModel()
        {

        }

        /// <summary>
        /// The priority for this result.
        /// </summary>
        public int ResultPriority
        {
            get;
            set;
        }

        /// <summary>
        /// Indexes into a collection of ChildCareModel objects
        /// that are included in this route.
        /// </summary>
        public IEnumerable<int> ChildCareIndices
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the plan for dropping a child off at
        /// a care service.  This or PickUpPlan may be null,
        /// but not both.
        /// </summary>
        public ItineraryModel DropOffPlan
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the plan for picking a child up from
        /// a care service.  This or DropOffPlan may be null,
        /// but not both.
        /// </summary>
        public ItineraryModel PickUpPlan
        {
            get;
            set;
        }
    }
}