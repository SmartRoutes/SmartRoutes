using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Itinerary
{
    public class PickUpAction : IChildItineraryAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PickUpAction()
        {
            this.ChildIndices = new List<int>();
        }

        /// <summary>
        /// Convenience constructor.
        /// </summary>
        /// <param name="childIndices">The child indices in the original
        /// query used in this action.</param>
        /// <param name="serviceName">The name of the service for this action.</param>
        public PickUpAction(IEnumerable<int> childIndices, string serviceName)
        {
            this.ChildIndices = childIndices;
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// The type of action (used for serialization).
        /// </summary>
        public ItineraryActionType Action
        {
            get
            {
                return ItineraryActionType.PickUp;
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
        /// The name of the service to pick the children up from.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }
    }
}