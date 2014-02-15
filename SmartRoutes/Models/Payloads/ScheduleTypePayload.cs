using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    /// <summary>
    /// Payload object representing the type of schedule to search for.
    /// </summary>
    public class ScheduleTypePayload
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ScheduleTypePayload()
        {

        }

        /// <summary>
        /// Flag indicating if the search should optimize for dropping a child of.
        /// </summary>
        public bool DropOffChecked
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating if the search should optimize for picking up a child.
        /// </summary>
        public bool PickUpChecked
        {
            get;
            set;
        }
    }
}