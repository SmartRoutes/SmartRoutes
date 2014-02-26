using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartRoutes.Demo.OdjfsDatabase.Model;

namespace SmartRoutes.Support
{
    public class AgeGroup
    {
        public string Name { get; set; }
        public Func<DetailedChildCare, bool> Validate { get; set; } 
    }
}