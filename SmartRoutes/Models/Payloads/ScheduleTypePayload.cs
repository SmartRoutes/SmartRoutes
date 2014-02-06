using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    public class ScheduleTypePayload
    {
        public ScheduleTypePayload()
        {

        }

        public bool DropOffChecked
        {
            get;
            set;
        }

        public bool PickUpChecked
        {
            get;
            set;
        }
    }
}