using System;
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
        /// The type of the itinerary.  Used for serialization.
        /// </summary>
        public abstract ItineraryType Type
        {
            get;
        }

        /// <summary>
        /// A collection of route identifiers (like route numbers) that are
        /// ordered by their usage in the route.
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
            this.actions.Push(action);
        }

        private Stack<IItineraryAction> actions = new Stack<IItineraryAction>();
    }
}