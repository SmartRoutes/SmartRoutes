using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Itinerary
{
    /// <summary>
    /// The type of action (used for serialization).
    /// </summary>
    public enum ItineraryActionType
    {
        DepartFromAddress = 0,
        ArriveAtAddress,
        DropOff,
        PickUp,
        BoardBus,
        ExitBus
    }

    public interface IItineraryAction
    {
        /// <summary>
        /// The type of action (used for serialization).
        /// </summary>
        ItineraryActionType Action
        {
            get;
        }
    }
}