using System;
using System.Collections.Generic;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public class DayCampTemplate : ChildCareTemplate<DayCamp>
    {
        private static readonly IDictionary<string, Func<DayCamp, string>> DefaultDetails = new Dictionary<string, Func<DayCamp, string>>
        {
            {"Registration Status", c => c.RegistrationStatus},
            {"Owner", c => c.Owner},
            {"Registration Begin Date", c => c.RegistrationBeginDate},
            {"Registration End Date", c => c.RegistrationEndDate},
        };

        public DayCampTemplate() : base(DefaultDetails)
        {
            Model.Address = "Address";

            Model.Owner = "Owner";
            Model.RegistrationBeginDate = "RegistrationBeginDate";
            Model.RegistrationEndDate = "RegistrationEndDate";
            Model.RegistrationStatus = "RegistrationStatus";
        }
    }
}