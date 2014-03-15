using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class SearchResultsStatus
    {
        /// <summary>
        /// Codes to be used by the client to indicate the success
        /// or failure of the search.
        /// </summary>
        public enum StatusCode
        {
            ResultsOk = 0,
            /// <summary>
            /// Indicates a failure with location transformation.
            /// </summary>
            GeocodeFail
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">The status code for the search.</param>
        public SearchResultsStatus(StatusCode code)
            : this(code, string.Empty)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">The status code for the search.</param>
        /// <param name="message">An error message to pass to the client.</param>
        public SearchResultsStatus(StatusCode code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        public StatusCode Code
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}