using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    public class ChildCareSearchQueryPayload
    {
        public ChildCareSearchQueryPayload()
        {

        }

        public IEnumerable<ChildInformationPayload> ChildInformation
        {
            get;
            set;
        }

        public ScheduleTypePayload ScheduleType
        {
            get;
            set;
        }

        public IEnumerable<AccreditationPayload> Accreditations
        {
            get;
            set;
        }

        public IEnumerable<ServiceTypePayload> ServiceTypes
        {
            get;
            set;
        }
    }
}