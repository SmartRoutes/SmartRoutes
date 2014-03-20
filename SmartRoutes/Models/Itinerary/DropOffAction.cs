using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Itinerary
{
    public class DropOffAction : IChildItineraryAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropOffAction()
        {
            this.ChildIndices = new List<int>();
        }

        /// <summary>
        /// Convenience constructor.
        /// </summary>
        /// <param name="childIndices">The indices of children included in this action.</param>
        /// <param name="serviceName">The name of the service used in this action.</param>
        /// <param name="childCareId">The ID of the associate child care.</param>
        public DropOffAction(IEnumerable<int> childIndices, string serviceName, int childCareId)
        {
            this.ChildIndices = childIndices;
            this.ServiceName = serviceName;
            this.ChildCareId = childCareId;
        }

        /// <summary>
        /// The type of action (used for serialization).
        /// </summary>
        public ItineraryActionType Action
        {
            get
            {
                return ItineraryActionType.DropOff;
            }
        }

        /// <summary>
        /// This collection should index the original search query.
        /// i.e. if the query contained 3 children, this collection should
        /// indicate which children should be included in this action.
        /// </summary>
        public IEnumerable<int> ChildIndices
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the child care service.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of the associated child care service.
        /// </summary>
        public int ChildCareId { get; set; }
    }
}