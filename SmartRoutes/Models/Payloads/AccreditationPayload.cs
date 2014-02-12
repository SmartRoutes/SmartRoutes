using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    /// <summary>
    /// The payload received from the client that indicates
    /// if an accreditation is selected.
    /// </summary>
    public class AccreditationPayload
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AccreditationPayload()
        {

        }

        /// <summary>
        /// Name of the accreditation.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating the checked state for the accreditation.
        /// </summary>
        public bool Checked
        {
            get;
            set;
        }
    }
}