using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Represents the "steps" that a user would take
    /// to get from a location, to child care(s), and 
    /// on to the final destination.
    /// </summary>
    public class ItineraryModel
    {
        // TODO: figure out what this should look like
        // It may be worth having a collection of "steps"
        // that are flagged with a type from an enumeration
        // or something similar.  This would then let the client
        // interpret the data to display it.

        /* i.e. (incomplete example)
         * enum stepType { origin, enterBus, exitBus, childCare, destination }
         * 
         * steps[0] = stepType[origin], <name/address/something?>
         * steps[1] = steptype[enterBus], <name/address/something>
         * steps[2] = stepType[exitBus], <descriptor>
         * steps[3] = stepType[childCare], <descriptor>
         * steps[4] = stepType[enterBus], <descriptor>
         * steps[5] = stepType[exitBus], <descriptor>
         * steps[6] = stepType[destination], <descriptor>
         * 
         * In all cases, descriptor may be the name of something, an address,
         * or some other identifying string.
        */


        /// <summary>
        /// Constructor.
        /// </summary>
        public ItineraryModel()
        {

        }

    }
}