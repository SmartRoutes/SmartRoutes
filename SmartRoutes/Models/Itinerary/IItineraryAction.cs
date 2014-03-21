using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartRoutes.Model;

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

    public interface IItineraryAction : ILocation
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