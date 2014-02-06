using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    public class ServiceTypePayload
    {
        public ServiceTypePayload()
        {

        }

        public string Name
        {
            get;
            set;
        }

        public bool Checked
        {
            get;
            set;
        }
    }
}