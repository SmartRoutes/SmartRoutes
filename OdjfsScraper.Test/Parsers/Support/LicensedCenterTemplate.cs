﻿using System;
using System.Linq;
using RazorEngine.Templating;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace OdjfsScraper.Test.Parsers.Support
{
    public class LicensedCenterTemplate : BaseDetailedChildCareHomeTemplate<LicensedCenter>
    {
        public LicensedCenterTemplate() : base(new LicensedCenter())
        {
        }
    }
}