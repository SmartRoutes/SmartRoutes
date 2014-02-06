using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    public class AccreditationPayload
    {
        public AccreditationPayload()
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