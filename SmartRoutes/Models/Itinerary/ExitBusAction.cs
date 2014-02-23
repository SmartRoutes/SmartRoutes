using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SmartRoutes.Models.Itinerary
{
    public class ExitBusAction : IItineraryAction
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
        /// <param name="exitTime">The time at which to exit the bus.</param>
        /// <param name="stopName">The name of the stop to exit the bus at.</param>
        public ExitBusAction(DateTime exitTime, string stopName)
        {
            this.ExitTime = exitTime;
            this.StopName = stopName;
        }

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
    }
}