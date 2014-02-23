using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SmartRoutes.Models.Itinerary
{
    public class BoardBusAction : IItineraryAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BoardBusAction()
        {
            this.BoardTime = DateTime.Now;
        }

        /// <summary>
        /// Convenience constructor.
        /// </summary>
        /// <param name="routeNumber">The route number to board.</param>
        /// <param name="boardTime">The time at which to board the bus.</param>
        /// <param name="stopName">The name of the stop to board the bus at.</param>
        public BoardBusAction(string routeNumber, DateTime boardTime, string stopName)
        {
            this.RouteNumber = routeNumber;
            this.BoardTime = boardTime;
            this.StopName = stopName;
        }

        /// <summary>
        /// The type of action (used for serialization).
        /// </summary>
        public ItineraryActionType Action
        {
            get
            {
                return ItineraryActionType.BoardBus;
            }
        }

        /// <summary>
        /// The identification number for the route.
        /// </summary>
        public string RouteNumber
        {
            get;
            set;
        }

        /// <summary>
        /// The time to board the bus at.  This is not serialized.
        /// </summary>
        [ScriptIgnore]
        public DateTime BoardTime
        {
            get;
            set;
        }

        /// <summary>
        /// The serialization property for the time to board the bus.
        /// </summary>
        public string Time
        {
            get
            {
                return string.Format("{0:t}", this.BoardTime);
            }
        }

        /// <summary>
        /// The name of the stop to board the bus at.
        /// </summary>
        public string StopName
        {
            get;
            set;
        }
    }
}