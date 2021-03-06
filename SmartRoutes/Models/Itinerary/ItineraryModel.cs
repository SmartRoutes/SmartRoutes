﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartRoutes.Models.Itinerary;

namespace SmartRoutes.Models.Itinerary
{
    /// <summary>
    /// Represents the "steps" that a user would take
    /// to get from a location, to child care(s), and 
    /// on to the final destination.
    /// </summary>
    public abstract class ItineraryModel
    {
        public enum ItineraryType
        {
            DropOff = 0,
            PickUp
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ItineraryModel()
        {

        }

        /// <summary>
        /// The duration of the itinerary, i.e. the time spent to get from
        /// the departure address to the destination address.
        /// </summary>
        public TimeSpan PathCost
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the itinerary.  Used for serialization.
        /// </summary>
        public abstract ItineraryType Type
        {
            get;
        }

        /// <summary>
        /// A collection of route numbers and child cares that are
        /// ordered by their usage in the itinerary.
        /// TODO: rename this to "Summary"
        /// </summary>
        public IEnumerable<string> Routes
        {
            get;
            set;
        }

        /// <summary>
        /// The ordered list of actions involved in the itinerary.
        /// </summary>
        public IEnumerable<IItineraryAction> ItineraryActions
        {
            get
            {
                return this.actions.AsEnumerable();
            }
        }

        /// <summary>
        /// Adds an action to the end of the model's action collection.
        /// </summary>
        /// <param name="action"></param>
        public void AddAction(IItineraryAction action)
        {
            this.actions.Add(action);
        }

        private IList<IItineraryAction> actions = new List<IItineraryAction>();
    }
}