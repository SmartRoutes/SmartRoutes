using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SmartRoutes.Model;

namespace SmartRoutes.Models.Itinerary
{
    public class ExitBusAction : IBusAction
    {
        /// <summary>
        /// Constructor;
        /// </summary>
        public ExitBusAction()
        {
            this.ExitTime = DateTime.Now;
        }

        /// <summary>
        /// Convenience constructor.
        /// </summary>
        /// <param name="location">The location of the stop to exit at.</param>
        /// <param name="routeNumber">The route number of the board to exit.</param>
        /// <param name="exitTime">The time at which to exit the bus.</param>
        /// <param name="stopName">The name of the stop to exit the bus at.</param>
        /// <param name="stopTimeId">The ID of the associated GTFS stop time.</param>
        /// <param name="routePath">The </param>
        public ExitBusAction(ILocation location, string routeNumber, DateTime exitTime, string stopName, int stopTimeId, IEnumerable<Location> routePath)
        {
            this.Latitude = location.Latitude;
            this.Longitude = location.Longitude;
            this.RouteNumber = routeNumber;
            this.ExitTime = exitTime;
            this.StopName = stopName;
            this.StopTimeId = stopTimeId;
            this.RoutePath = routePath;
        }

        /// <summary>
        /// A sequence of latitude-longitude pairs that show the path of the bus.
        /// </summary>
        public IEnumerable<Location> RoutePath { get; set; }

        /// <summary>
        /// The type of action (used for serialization).
        /// </summary>
        public ItineraryActionType Action
        {
            get
            {
                return ItineraryActionType.ExitBus;
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
        /// The time to exit the bus at.  This property is not serialized.
        /// </summary>
        [ScriptIgnore]
        public DateTime ExitTime
        {
            get;
            set;
        }

        /// <summary>
        /// Serialization property for the time to exit the bus.
        /// </summary>
        public string Time
        {
            get
            {
                return string.Format("{0:t}", this.ExitTime);
            }
        }

        /// <summary>
        /// The name of the stop to exit at.
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