using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    public class AddressPayload
    {
        public AddressPayload()
        {

        }

        public string Address
        {
            get;
            set;
        }

        public string AddressLine2
        {
            get;
            set;
        }

        public string City
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string ZipCode
        {
            get;
            set;
        }

    }
}