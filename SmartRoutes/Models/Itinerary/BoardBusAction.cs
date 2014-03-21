using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Models.Itinerary
{
    public class BoardBusAction : IBusAction
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
        /// <param name="location">The location of the bus stop.</param>
        /// <param name="routeNumber">The route number to board.</param>
        /// <param name="boardTime">The time at which to board the bus.</param>
        /// <param name="stopName">The name of the stop to board the bus at.</param>
        /// <param name="stopTimeId">The ID of the associated GTFS stop time.</param>
        public BoardBusAction(ILocation location, string routeNumber, DateTime boardTime, string stopName, int stopTimeId)
        {
            this.Latitude = location.Latitude;
            this.Longitude = location.Longitude;
            this.RouteNumber = routeNumber;
            this.BoardTime = boardTime;
            this.StopName = stopName;
            this.StopTimeId = stopTimeId;
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

        /// <summary>
        /// The ID of the associate GTFS stop time.
        /// </summary>
        public int StopTimeId { get; set; }

        /// <summary>
        /// The latitude of the bus stop.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude of the bus stop.
        /// </summary>
        public double Longitude { get; set; }
    }
}