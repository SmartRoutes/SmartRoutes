using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Map
{
    public class ChildCareTimeLocationModel : TimeLocationModel
    {
        public ChildCareModel ChildCare { get; set; }
    }
}