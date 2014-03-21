using SmartRoutes.Model;

namespace SmartRoutes.Models.Itinerary
{
    public interface IBusAction : IItineraryAction
    {
        /// <summary>
        /// The identification number for the route.
        /// </summary>
        string RouteNumber { get; set; }

        /// <summary>
        /// The name of the stop to board the bus at.
        /// </summary>
        string StopName { get; set; }

        /// <summary>
        /// The ID of the associate GTFS stop time.
        /// </summary>
        int StopTimeId { get; set; }
    }
}