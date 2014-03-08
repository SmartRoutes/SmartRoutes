using System.Collections.Generic;

namespace SmartRoutes.Models.Itinerary
{
    public interface IChildItineraryAction : IItineraryAction
    {
        /// <summary>
        /// This collection should index the original search query.
        /// i.e. if the query contained 3 children, this collection should
        /// indicate which children should be included in this action.
        /// </summary>
        IEnumerable<int> ChildIndices
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the child care service.
        /// </summary>
        string ServiceName
        {
            get;
            set;
        }
    }
}