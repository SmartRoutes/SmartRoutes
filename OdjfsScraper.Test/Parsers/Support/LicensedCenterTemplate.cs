using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public class LicensedCenterTemplate : DetailedChildCareHomeTemplate<LicensedCenter>
    {
        public LicensedCenterTemplate() : base(Enumerable.Empty<KeyValuePair<string, Func<LicensedCenter, string>>>())
        {
        }
    }
}