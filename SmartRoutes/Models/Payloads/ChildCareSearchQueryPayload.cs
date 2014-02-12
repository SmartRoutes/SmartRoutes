using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    /// <summary>
    /// Payload received from the client for a child care search.
    /// </summary>
    public class ChildCareSearchQueryPayload
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChildCareSearchQueryPayload()
        {

        }

        /// <summary>
        /// Collection of information describing the children to use in a search.
        /// </summary>
        public IEnumerable<ChildInformationPayload> ChildInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Object representing the type of schedule and addresses to use in a search.
        /// </summary>
        public ScheduleTypePayload ScheduleType
        {
            get;
            set;
        }

        /// <summary>
        /// A collection of information about the accreditations to include
        /// in a search.
        /// </summary>
        public IEnumerable<AccreditationPayload> Accreditations
        {
            get;
            set;
        }

        /// <summary>
        /// A collection of information about the service types to include in a search.
        /// </summary>
        public IEnumerable<ServiceTypePayload> ServiceTypes
        {
            get;
            set;
        }
    }
}